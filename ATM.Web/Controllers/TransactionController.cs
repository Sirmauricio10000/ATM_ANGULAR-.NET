using ATM.Data;
using ATM.Logic;
using ATM.Logic.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATM.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionlService service;

        public TransactionController(TransactionlService service)
        {
            this.service = service;
        }

        [HttpGet("AmountAvailable")]
        public async Task<IEnumerable<BillAmount>> AmountAvailable()
        {
            var result = await service.GetAmounts();
            return result.ToList();
        }

        [HttpGet("WithdrawalOptions")]
        public async Task<IEnumerable<WithdrawalOption>> GetOptionForAmount(int amount)
        {
            var result = await service.GetWithdrawalAvailableOptions(amount);

            return result.Options.OrderBy(x => x.Score)
                .Take(15)
                .ToList();
        }

        [HttpGet("Withdraw")]
        public async Task<TransactionResult> Withdraw(int amount)
        {
            return await service.Withdraw(amount);
        }
    }

}