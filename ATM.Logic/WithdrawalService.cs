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

namespace ATM.Logic
{
    public class WithdrawalService
    {
        private readonly ATMContext context;

        public WithdrawalService(ATMContext context)
        {
            this.context = context;
        }


        public async Task<TransactionResult> Withdraw(int ammount)
        {
            var bills = await GetAmmounts();
            var options = GenerateWithdrawalOptions(bills, ammount);

            if(options.Result == WithdrawalResult.AvailableFunds)
            {
                var chosenOption = options.Options.OrderBy(o => o.Score).First();
                var newBillAmmounts = bills.Select(b => new Bill() {
                    Denomination = b.Denomination,
                    Quantity = b.Quantity - chosenOption.Bills.Single(x => x.Denomination == b.Denomination).Quantity
                }).ToList();

                context.UpdateRange(newBillAmmounts);
                await context.SaveChangesAsync();
                return new TransactionResult(TransactionStatus.Success, chosenOption.Bills);
            }

            return new TransactionResult(TransactionStatus.UnavailableFunds);
        }

        public async Task<ResultWithdrawalOption> GetWithdrawalAvailableOptions(int ammount)
        {
            var bills = await GetAmmounts();
            return GenerateWithdrawalOptions(bills, ammount);
        }

        private async Task<IEnumerable<BillAmmount>> GetAmmounts()
        {
            return (await context.Bills.ToListAsync())
                .Select(x => new BillAmmount(x.Denomination, x.Quantity));
        }


        private ResultWithdrawalOption GenerateWithdrawalOptions(IEnumerable<BillAmmount> bills, int ammount)
        {
            var availableAmmount = bills.Sum(b => b.Ammount);

            if (availableAmmount < ammount)
                return new ResultWithdrawalOption();


            var combinations = new AmmountManager(bills.OrderByDescending(x => x.Denomination));
            var options = new List<IEnumerable<BillAmmount>>();
            var anyOptions = true;


            while (anyOptions)
            {
                var (withdrawalOption, remainer) = GenerateWithdrawalOption(combinations, ammount);

                combinations.SetAmount(withdrawalOption);

                if (remainer == 0)
                {
                    options.Add(withdrawalOption);
                }

                var reduceBill = combinations.Take(0..^1).Where(x => x.Quantity != 0)
                    .Select<BillAmmount, BillAmmount?>(x => x).LastOrDefault();


                if (reduceBill is BillAmmount bill)
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

        private (IEnumerable<BillAmmount> bills, int remainer) GenerateWithdrawalOption(IEnumerable<BillAmmount> denominationsAvailable, int ammount)
        {
            int remainder = ammount;

            var bills = new List<BillAmmount>();

            var denominations = denominationsAvailable
                .Where(x => x.Denomination <= ammount && x.Quantity > 0)
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

                bills.Add(new BillAmmount(item.Denomination, quotientResult));

                if (remainder == 0)
                {
                    return (bills.ToImmutableList(), 0);
                }
            }

            return (bills.ToImmutableList(), remainder);
        }
    }
}
