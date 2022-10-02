using ATM.Core.Model;
using ATM.Logic.Model;

namespace ATM.Web.Model
{
    public class WithdrawAmount
    {
        public int Amount { get; set; }
    }

    public class TransactionData
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime DateTime { get; set; }
        public TransactionType Type { get; set; }
        public IEnumerable<BillAmount> Amount { get; set; } = new List<BillAmount>();
    }
}
