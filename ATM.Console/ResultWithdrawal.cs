using System.Collections.Immutable;

public readonly struct ResultWithdrawal
{
    public ResultTransaction Result { get; init; }

    public IEnumerable<WithdrawalOption> Options { get; init; }

    public ResultWithdrawal(IEnumerable<IEnumerable<Bill>> bills) : this(ResultTransaction.AvailableFunds, bills)
    {

    }

    public ResultWithdrawal(ResultTransaction result, IEnumerable<IEnumerable<Bill>>? options = null, IEnumerable<Bill>? denominations = null)
    {
        Result = result;

        if (options != null && denominations != null)
        {
            decimal maxQuantityBill = options.Select(b => b.Sum(i => i.Quantity)).Max();
            Options = options.Select(option => BuildOption(option, denominations, maxQuantityBill)).ToImmutableList();
        }
        else
        {
            Options = Enumerable.Empty<WithdrawalOption>().ToImmutableList();
        }
       
    }

    private static WithdrawalOption BuildOption(IEnumerable<Bill> option, IEnumerable<Bill> denominations, decimal maxQuantityBill)
    {
        decimal scoreByMaxBill = (maxQuantityBill == 0) ? 0 : (option.Sum(b => b.Quantity) / maxQuantityBill);
        decimal scoreByDenomination = GetScoreByDenomination(option, denominations);
        return new WithdrawalOption(option, scoreByMaxBill + scoreByDenomination);
    }

    private static decimal GetScoreByDenomination(IEnumerable<Bill> option, IEnumerable<Bill> denominations)
    {
        var missingDenomination = GetMissingDenomination(option, denominations);
        decimal quantityBills = option.Sum(x => x.Quantity);

        return option.Concat(missingDenomination).Select(b =>
        {
            decimal available = denominations.FirstOrDefault(i => i.Denomination == b.Denomination).Quantity;
            decimal ratioDenomination = (available == 0) ? 0 : (b.Quantity / available);
            decimal ratioDistribution = (b.Quantity / quantityBills) < 0.1M ? 0.1M : 0;
            return ratioDenomination * ratioDenomination * 0.8M + ratioDistribution * 0.2M;
        }).Sum();
    }

    private static IEnumerable<Bill> GetMissingDenomination(IEnumerable<Bill> option, IEnumerable<Bill> denominations)
    {
        return denominations
                   .ExceptBy(option.Select(x => x.Denomination), b => b.Denomination)
                   .Select(x => new Bill(x.Denomination, 0));
    }

    
}
