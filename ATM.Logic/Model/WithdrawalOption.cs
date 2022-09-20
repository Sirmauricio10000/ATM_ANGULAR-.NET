namespace ATM.Logic.Model;

public struct WithdrawalOption
{
    public IEnumerable<BillAmmount> Bills { get; init; }
    public decimal Score { get; init; }


    public WithdrawalOption(IEnumerable<BillAmmount> bills, decimal score)
    {
        Bills = bills;
        Score = score;
    }
}
