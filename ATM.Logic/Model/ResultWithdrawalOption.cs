using System.Collections.Immutable;

namespace ATM.Logic.Model;

public readonly struct ResultWithdrawalOption
{
    public WithdrawalResult Result { get; init; }

    public IEnumerable<WithdrawalOption> Options { get; init; }

    public ResultWithdrawalOption(IEnumerable<IEnumerable<BillAmmount>> options, IEnumerable<BillAmmount> ammountAvailable)
    {

        if (options != null && ammountAvailable != null)
        {
            decimal maxQuantityBill = options.Select(b => b.Sum(i => i.Quantity)).Max();
            Options = options.Select(option => BuildOption(option, ammountAvailable, maxQuantityBill)).ToImmutableList();
            Result = WithdrawalResult.AvailableFunds;
        }
        else
        {
            Options = Enumerable.Empty<WithdrawalOption>().ToImmutableList();
            Result = WithdrawalResult.UnavailableFunds;
        }
       
    }

    private static WithdrawalOption BuildOption(IEnumerable<BillAmmount> option, IEnumerable<BillAmmount> denominations, decimal maxQuantityBill)
    {
        decimal scoreByMaxBill = (maxQuantityBill == 0) ? 0 : (option.Sum(b => b.Quantity) / maxQuantityBill);
        decimal scoreByDenomination = GetScoreByDenomination(option, denominations);
        return new WithdrawalOption(option, scoreByMaxBill + scoreByDenomination);
    }

    private static decimal GetScoreByDenomination(IEnumerable<BillAmmount> option, IEnumerable<BillAmmount> denominations)
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

    private static IEnumerable<BillAmmount> GetMissingDenomination(IEnumerable<BillAmmount> option, IEnumerable<BillAmmount> denominations)
    {
        return denominations
                   .ExceptBy(option.Select(x => x.Denomination), b => b.Denomination)
                   .Select(x => new BillAmmount(x.Denomination, 0));
    }

    
}
