using ATM.Data;
using ATM.Logic;
using ATM.Logic.Model;
using ATM.Web.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATM.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionlService service;

        public TransactionController(TransactionlService service)
        {
            this.service = service;
        }

        [HttpGet(nameof(GetAmountAvailable))]
        public async Task<IEnumerable<BillAmount>> GetAmountAvailable()
        {
            var result = await service.GetAmounts();
            return result.ToList();
        }

        [HttpGet(nameof(GetTransactions))]
        public async Task<IEnumerable<TransactionData>> GetTransactions()
        {
            var result = await service.GetTransactionsAsync();
            return result.Select(x => new TransactionData()
            {
                Id = x.Id,
                Type = x.Type,
                DateTime = x.DateTime,
                Amount = x.Amount.Select(a => new BillAmount(a.Denomination, a.Quantity))
            });
        }


        [HttpGet(nameof(GetOptionForAmount))]
        public async Task<IEnumerable<WithdrawalOption>> GetOptionForAmount(int amount)
        {
            var result = await service.GetWithdrawalAvailableOptions(amount);

            return result.Options.OrderBy(x => x.Score)
                .Take(15)
                .ToList();
        }

        [HttpPost(nameof(Withdraw))]
        public async Task<TransactionResult> Withdraw(WithdrawAmount withdraw)
        {
            return await service.Withdraw(withdraw.Amount);
        }

        [HttpPost(nameof(UpdateAmounts))]
        public async Task<IActionResult> UpdateAmounts(IEnumerable<BillAmount> amounts)
        {
            await service.UpdateReserve(amounts);
            return Ok();
        }

        [HttpPost(nameof(DepositAmounts))]
        public async Task<IActionResult> DepositAmounts(IEnumerable<BillAmount> amounts)
        {
            await service.DepositAmounts(amounts);
            return Ok();
        }

    }







}