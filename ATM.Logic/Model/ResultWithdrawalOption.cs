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

    private static WithdrawalOption BuildOption(IEnumerable<BillAmount> option, IEnumerable<BillAmount> amountAvailable, decimal maxQuantityBill)
    {
        decimal scoreByMaxBill = (maxQuantityBill == 0) ? 0 : (option.Sum(b => b.Quantity) / maxQuantityBill);
        decimal scoreByDenomination = GetScoreByDenomination(option, amountAvailable);
        return new WithdrawalOption(option, scoreByMaxBill + scoreByDenomination);
    }

    private static decimal GetScoreByDenomination(IEnumerable<BillAmount> option, IEnumerable<BillAmount> availableAmounts)
    {
        var missingDenomination = GetMissingDenomination(option, availableAmounts);
        decimal quantityBills = option.Sum(x => x.Quantity);
        decimal totalAmount = option.Sum(x => x.Amount);

        return option.Concat(missingDenomination).Select(b =>
        {
            decimal ratioTotalAmount = (b.Quantity * b.Denomination / totalAmount);
            decimal available = availableAmounts.FirstOrDefault(i => i.Denomination == b.Denomination).Quantity;
            decimal ratioDenomination = (available == 0) ? 0 : (b.Quantity / available);
            decimal ratioDistribution = ratioTotalAmount < 0.05M ? 0.05M - ratioTotalAmount : 0;
            return ratioDenomination.Pow(3) + ratioDistribution;
        }).Sum();
    }

    private static IEnumerable<BillAmount> GetMissingDenomination(IEnumerable<BillAmount> option, IEnumerable<BillAmount> denominations)
    {
        return denominations
                   .ExceptBy(option.Select(x => x.Denomination), b => b.Denomination)
                   .Select(x => new BillAmount(x.Denomination, 0));
    }
    
}


static class DecimalExtensions
{
    public static decimal Pow(this decimal value, decimal pow)
    {
        return ((decimal)Math.Pow(decimal.ToDouble(value), 3));
    }
}
