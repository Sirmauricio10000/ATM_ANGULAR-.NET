using System.Collections.Immutable;

namespace ATM.Logic.Model;

public readonly struct ResultWithdrawalOption
{
    public WithdrawalResult Result { get; init; }

    public IEnumerable<WithdrawalOption> Options { get; init; }

    public ResultWithdrawalOption()
    {
        Result = WithdrawalResult.UnavailableFunds;
        Options = Enumerable.Empty<WithdrawalOption>();
    }


    public ResultWithdrawalOption(IEnumerable<IEnumerable<BillAmount>> options, IEnumerable<BillAmount> amountAvailable)
    {

        if (options != null && amountAvailable != null)
        {
            decimal maxQuantityBill = options.Select(b => b.Sum(i => i.Quantity)).Max();
            Options = options.Select(option => BuildOption(option, amountAvailable, maxQuantityBill)).ToImmutableList();
            Result = WithdrawalResult.AvailableFunds;
        }
        else
        {
            Options = Enumerable.Empty<WithdrawalOption>().ToImmutableList();
            Result = WithdrawalResult.UnavailableFunds;
        }
       
    }

    private static WithdrawalOption BuildOption(IEnumerable<BillAmount> option, IEnumerable<BillAmount> denominations, decimal maxQuantityBill)
    {
        decimal scoreByMaxBill = (maxQuantityBill == 0) ? 0 : (option.Sum(b => b.Quantity) / maxQuantityBill);
        decimal scoreByDenomination = GetScoreByDenomination(option, denominations);
        return new WithdrawalOption(option, scoreByMaxBill + scoreByDenomination);
    }

    private static decimal GetScoreByDenomination(IEnumerable<BillAmount> option, IEnumerable<BillAmount> denominations)
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

    private static IEnumerable<BillAmount> GetMissingDenomination(IEnumerable<BillAmount> option, IEnumerable<BillAmount> denominations)
    {
        return denominations
                   .ExceptBy(option.Select(x => x.Denomination), b => b.Denomination)
                   .Select(x => new BillAmount(x.Denomination, 0));
    }

    
}
