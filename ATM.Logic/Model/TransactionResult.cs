using ATM.Core.Model;

namespace ATM.Logic.Model;

public class TransactionResult
{
    public TransactionResult()
    {

    }

    public TransactionResult(TransactionStatus status)
    {
        Status = status;
        Amount = Enumerable.Empty<BillAmount>();
    }

    public TransactionResult(TransactionStatus status, IEnumerable<BillAmount> amount)
    {
        Status = status;
        Amount = amount;
    }

    public TransactionStatus Status { get; set; }

    public IEnumerable<BillAmount> Amount { get; set; }
}
