using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;

int retiro = 200;


var reserves = new Amount(new[] {
    //new Bill(5, 10),
    new Bill(10, 40),
    new Bill(20, 30),
    new Bill(50, 25),
    new Bill(100, 10),
});

var disponible = reserves.Total;

Console.WriteLine(disponible);


var resultTransaction = WithdrawalOptions(reserves, retiro);

if (resultTransaction.Result == ResultTransaction.AvailableFunds)
{
    foreach (var option in resultTransaction.Options.OrderBy(x => x.Score).Take(10))
    {
        foreach (var bill in option.Bills)
        {
            Console.WriteLine($"d: {bill.Denomination}, q: {bill.Quantity}");
        }

        Console.WriteLine();
    }

}


(IEnumerable<Bill> withdrawalOption, int remainer) GetWithdrawalOption(Amount amount, int withdrawal)
{
    int remainder = withdrawal;

    var bills = new List<Bill>();

    var denominations = amount
        .Where(x => x.Denomination <= retiro && x.Quantity > 0)
        .OrderByDescending(x => x.Denomination);

    foreach (var item in denominations)
    {
        (int quotientResult, int remainderResult) = Math.DivRem(remainder, item.Denomination);


        if (quotientResult > item.Quantity)
        {
            quotientResult = item.Quantity;
            remainder -= quotientResult * item.Denomination;
        }
        else
        {
            remainder = remainderResult;
        }

        bills.Add(new Bill(item.Denomination, quotientResult));

        if (remainder == 0)
        {
            return (bills.ToImmutableList(), 0);
        }
    }

    return (bills.ToImmutableList(), remainder);
}


ResultWithdrawal WithdrawalOptions(Amount amount, int withdrawal)
{
    if (amount.Total < withdrawal)
        return new ResultWithdrawal(ResultTransaction.UnavailableFunds);

    var combinations = new Amount(amount.OrderByDescending(x => x.Denomination));
    
    var options = new List<IEnumerable<Bill>>();
    var anyOptions = true;


    while (anyOptions)
    {
        var (withdrawalOption, remainer) = GetWithdrawalOption(combinations, withdrawal);

        combinations.SetAmount(withdrawalOption);

        if (remainer == 0)
        {
            options.Add(withdrawalOption);
        }
        
        var reduceBill = combinations.Take(0..^1).Where(x => x.Quantity != 0)
            .Select<Bill, Bill?>(x => x).LastOrDefault();


        if(reduceBill is Bill bill)
        {
            var denominations = combinations.Select(x => x.Denomination).ToList();

            foreach (var denomination in denominations)
            {
                if (bill.Denomination == denomination)
                {
                    combinations[denomination] -= 1;
                }
                else if (bill.Denomination > denomination)
                {
                    combinations[denomination] = amount[denomination];
                }
            }
        }
        else
        {
            anyOptions = false;
        }
    }


    if(options.Any())
    {
        return new ResultWithdrawal(ResultTransaction.AvailableFunds, options, reserves.ToImmutableList());
    }

    return new ResultWithdrawal(ResultTransaction.UnavailableFunds);
}
