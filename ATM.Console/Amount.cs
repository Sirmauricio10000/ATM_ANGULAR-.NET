using System.Collections;
using System.Collections.Immutable;

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
