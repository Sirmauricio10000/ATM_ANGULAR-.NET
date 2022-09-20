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
    }

    public TransactionResult(TransactionStatus status, IEnumerable<BillAmmount> ammount)
    {
        Status = status;
        Ammount = ammount;
    }

    public TransactionStatus Status { get; set; }

    public IEnumerable<BillAmmount> Ammount { get; set; }
}
