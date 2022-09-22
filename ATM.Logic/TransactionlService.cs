using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Core.Model;
using ATM.Data;
using ATM.Logic.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ATM.Logic
{
    public class TransactionlService
    {
        private readonly ATMContext context;

        public TransactionlService(ATMContext context)
        {
            this.context = context;
        }


        public async Task<TransactionResult> Withdraw(int amount)
        {
            var bills = await GetAmounts();
            var options = GenerateWithdrawalOptions(bills, amount);

            if(options.Result == WithdrawalResult.AvailableFunds)
            {
                var chosenOption = options.Options.OrderBy(o => o.Score).First();
                var updateBillAmounts = GetUpdateBillAmounts(bills, chosenOption);
                var transaction = BuildTransaction(chosenOption);

                using (var transactionDb = await context.Database.BeginTransactionAsync())
                {
                    context.Add(transaction);
                    context.UpdateRange(updateBillAmounts);
                    await context.SaveChangesAsync();
                    transactionDb.Commit();
                }

                return new TransactionResult(TransactionStatus.Success, chosenOption.Bills);
            }

            return new TransactionResult(TransactionStatus.UnavailableFunds);
        }

        public async Task<ResultWithdrawalOption> GetWithdrawalAvailableOptions(int amount)
        {
            var bills = await GetAmounts();
            return GenerateWithdrawalOptions(bills, amount);
        }
        public async Task<IEnumerable<BillAmount>> GetAmounts()
        {
            return (await context.Bills.AsNoTracking().ToListAsync())
                .Select(x => new BillAmount(x.Denomination, x.Quantity));
        }
        public async Task UpdateReserve(IEnumerable<BillAmount> amounts)
        {
            await VerifySupportedBills(amounts);

            var newAmounts = amounts.GroupBy(x => x.Denomination)
                .Select(x => new Bill(x.Key, x.Sum(b => b.Quantity)))
                .ToList();

            context.UpdateRange(newAmounts);
            await context.SaveChangesAsync();
        }
        public async Task DepositAmounts(IEnumerable<BillAmount> amounts)
        {
            var bills = await GetAmounts();
            
            await VerifySupportedBills(amounts, bills.Select(x => x.Denomination));

            var newAmounts = amounts.GroupBy(x => x.Denomination)
                .Select(x => new Bill(x.Key, bills.Single(b => b.Denomination == x.Key).Quantity + x.Sum(b => b.Quantity)))
                .ToList();

            context.UpdateRange(newAmounts);
            await context.SaveChangesAsync();
        }


        private static List<Bill> GetUpdateBillAmounts(IEnumerable<BillAmount> bills, WithdrawalOption chosenOption)
        {
            return chosenOption.Bills.Select(b => new Bill()
            {
                Denomination = b.Denomination,
                Quantity = bills.Single(x => x.Denomination == b.Denomination).Quantity - b.Quantity
            }).ToList();
        }
        private static Transaction BuildTransaction(WithdrawalOption chosenOption)
        {
            var id = Guid.NewGuid().ToString();
            return new Transaction()
            {
                Id = id,
                Type = TransactionType.Withdrawal,
                DateTime = DateTime.Now,
                Amount = chosenOption.Bills.Select(x => new AmountTransaction()
                {
                    Denomination = x.Denomination,
                    Quantity = x.Quantity,
                    TransactionId = id
                }).ToList(),
            };
        }
        private async Task VerifySupportedBills(IEnumerable<BillAmount> requestedBills, IEnumerable<int>? billsSupported = null)
        {
            billsSupported ??= await context.Bills.Select(x => x.Denomination).ToListAsync();

            var unsupportedBills = requestedBills.DistinctBy(x => x.Denomination).Select(x => x.Denomination)
                .Except(billsSupported)
                .OrderBy(x => x)
                .ToList();

            if (unsupportedBills.Any())
            {
                var listString = string.Join(", ", unsupportedBills);
                throw new InvalidOperationException($"One or more bill not supported: ({listString})");
            }
        }
        private ResultWithdrawalOption GenerateWithdrawalOptions(IEnumerable<BillAmount> bills, int amount)
        {
            var availableAmount = bills.Sum(b => b.Amount);

            if (availableAmount < amount)
                return new ResultWithdrawalOption();


            var combinations = new AmountManager(bills.OrderByDescending(x => x.Denomination));
            var options = new List<IEnumerable<BillAmount>>();
            var anyOptions = true;


            while (anyOptions)
            {
                var (withdrawalOption, remainer) = GenerateWithdrawalOption(combinations, amount);

                combinations.SetAmount(withdrawalOption);

                if (remainer == 0)
                {
                    options.Add(withdrawalOption);
                }

                var reduceBill = combinations.Take(0..^1).Where(x => x.Quantity != 0)
                    .Select<BillAmount, BillAmount?>(x => x).LastOrDefault();


                if (reduceBill is BillAmount bill)
                {
                    var denominations = combinations.Select(x => x.Denomination).ToList();

                    foreach (var denomination in denominations)
                    {
                        if (bill.Denomination == denomination)
                        {
                            combinations[denomination] -= 1;
                        }
                        else if (bill.Denomination > denomination)
                        {
                            combinations[denomination] = bills.FirstOrDefault(x => x.Denomination == denomination).Quantity;
                        }
                    }
                }
                else
                {
                    anyOptions = false;
                }
            }


            if (options.Any())
            {
                return new ResultWithdrawalOption(options, bills);
            }

            return new ResultWithdrawalOption();
        }
        private (IEnumerable<BillAmount> bills, int remainer) GenerateWithdrawalOption(IEnumerable<BillAmount> denominationsAvailable, int amount)
        {
            int remainder = amount;

            var bills = new List<BillAmount>();

            var denominations = denominationsAvailable
                .Where(x => x.Denomination <= amount && x.Quantity > 0)
                .OrderByDescending(x => x.Denomination);

            foreach (var item in denominations)
            {
                (int quotientResult, int remainderResult) = Math.DivRem(remainder, item.Denomination);


                if (quotientResult > item.Quantity)
                {
                    quotientResult = item.Quantity;
                    remainder -= quotientResult * item.Denomination;
                }
                else
                {
                    remainder = remainderResult;
                }

                bills.Add(new BillAmount(item.Denomination, quotientResult));

                if (remainder == 0)
                {
                    return (bills.ToImmutableList(), 0);
                }
            }

            return (bills.ToImmutableList(), remainder);
        }

        
    }
}
