using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;

int retiro = 400;


var reserves = new Amount(new[] {
    //new Bill(5, 10),
    new Bill(10, 40),
    new Bill(20, 30),
    new Bill(50, 25),
    new Bill(100, 20),
});

var disponible = reserves.Total;

//if (retiro > disponible)
//    return;


Console.WriteLine(disponible);


var f = new[] { 8, 4 };
Console.WriteLine(String.Join(" - ", f[1..^1]));


var resultTransaction = WithdrawalOptions(reserves, retiro);

if (resultTransaction.Result == ResultTransaction.AvailableFunds)
{
    foreach (var option in resultTransaction.Options)
    {
        foreach (var bill in option.Bills)
        {
            Console.WriteLine($"d: {bill.Denomination}, q: {bill.Quantity}");
        }

        Console.WriteLine();
    }

}


(WithdrawalOption withdrawalOption, int remainer) GetWithdrawalOption(Amount amount, int withdrawal)
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
            return (new WithdrawalOption() { Bills = bills }, 0);
        }
    }

    return (new WithdrawalOption() { Bills = bills}, remainder);
}


ResultWithdrawal WithdrawalOptions(Amount amount, int withdrawal)
{
    if (amount.Total < withdrawal)
        return new ResultWithdrawal(ResultTransaction.UnavailableFunds);

    var combinations = new Amount(amount.OrderByDescending(x => x.Denomination));
    
    var options = new List<WithdrawalOption>();
    var anyOptions = true;


    while (anyOptions)
    {
        var (withdrawalOption, remainer) = GetWithdrawalOption(combinations, withdrawal);

        combinations.SetAmount(withdrawalOption.Bills);

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
        return new ResultWithdrawal(options);
    }

    return new ResultWithdrawal(ResultTransaction.UnavailableFunds);
}


public readonly struct Bill
{
    public int Denomination { get; init; }
    public int Quantity { get; init; }

    public int Total => Denomination * Quantity;

    public Bill(int denomination, int quantity)
    {
        if (denomination < 0)
            throw new ArgumentOutOfRangeException(nameof(denomination));

        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        Denomination = denomination;
        Quantity = quantity;
    }

    public Bill(int denomination) : this(denomination, 0)
    { }

}

public class Amount : IEnumerable<Bill>
{
    private readonly IDictionary<int, int> bills;

    public IEnumerable<Bill> Bills => bills.Select(x => new Bill(x.Key, x.Value))
        .ToImmutableList();

    public int Total => bills.Sum(x => x.Key * x.Value);

    public Amount()
    {
        bills = new Dictionary<int, int>();
    }

    public Amount(IEnumerable<Bill> bills)
    {
        this.bills = GroupAmount(bills)
            .ToDictionary(x => x.Denomination, x => x.Quantity);
    }

    public void SetAmount(IEnumerable<Bill> bills)
    {
        var groupAmount = GroupAmount(bills).ToList();
        var emptyAmount = this.bills.ExceptBy(groupAmount.Select(x => x.Denomination), x => x.Key)
            .Select(x => new Bill(x.Key, 0));

        foreach (var bill in groupAmount.Concat(emptyAmount))
        {
            SetAmount(bill);
        }
    }

    public void SetAmount(Bill bill)
    {
        bills[bill.Denomination] = bill.Quantity;
    }

    public int this[int denomination]
    {
        get
        {
            if (denomination < 0)
                throw new ArgumentOutOfRangeException(nameof(denomination));

            return bills[denomination];
        }

        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            bills[denomination] = value;
        }
    }


    public Amount Clone()
    {
        return new Amount(Bills);
    }

    private IEnumerable<Bill> GroupAmount(IEnumerable<Bill> bills)
    {
        return bills.GroupBy(x => x.Denomination)
            .Select(x => new Bill(x.Key, x.Sum(b => b.Quantity)));
    }

    public IEnumerator<Bill> GetEnumerator()
    {
        return Bills.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}


public struct ResultWithdrawal
{
    public ResultTransaction Result { get; init; }

    public IEnumerable<WithdrawalOption> Options { get; init; }

    public ResultWithdrawal(ResultTransaction result, IEnumerable<WithdrawalOption>? bills = null)
    {
        Result = result;
        Options = (bills ?? new List<WithdrawalOption>()).ToList().AsReadOnly();
    }

    public ResultWithdrawal(IEnumerable<WithdrawalOption> bills): this(ResultTransaction.AvailableFunds, bills)
    { }
}


public struct WithdrawalOption
{
    public IEnumerable<Bill> Bills { get; init; }


    public WithdrawalOption(IEnumerable<Bill> bills)
    {
        Bills = bills;
    }
}


public enum ResultTransaction
{
    NotSpecified,
    AvailableFunds,
    UnavailableFunds,
    InsufficientFunds
}


class UnavailableFunds : Exception
{
    public int Required { get; init; }
    public int Available { get; init; }

    public UnavailableFunds(int required, int available, string? message):
        this(required, available, message, null)
    {

    }

    public UnavailableFunds(int required, int available, string? message, Exception? innerException) : base(message, innerException)
    {
        Required = required;
        Available = available;
    }
}