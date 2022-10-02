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

    public TransactionResult(TransactionStatus status, IEnumerable<BillAmount>? amount = null, string message = "")
    {
        Status = status;
        Amount = amount ?? Enumerable.Empty<BillAmount>();
        Message = message;
    }

    public TransactionStatus Status { get; set; }

    public IEnumerable<BillAmount> Amount { get; set; }

    public string Message { get; set; }
}
