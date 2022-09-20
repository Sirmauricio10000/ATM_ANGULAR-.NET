public struct WithdrawalOption
{
    public IEnumerable<Bill> Bills { get; init; }
    public decimal Score { get; init; }


    public WithdrawalOption(IEnumerable<Bill> bills, decimal score)
    {
        Bills = bills;
        Score = score;
    }
}
