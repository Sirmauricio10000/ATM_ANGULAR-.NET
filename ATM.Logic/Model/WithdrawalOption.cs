namespace ATM.Logic.Model;

public struct WithdrawalOption
{
    public IEnumerable<BillAmount> Bills { get; init; }
    public decimal Score { get; init; }


    public WithdrawalOption(IEnumerable<BillAmount> bills, decimal score)
    {
        Bills = bills;
        Score = score;
    }
}
