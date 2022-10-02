using System.Collections;
using System.Collections.Immutable;

namespace ATM.Logic.Model;

public class AmountManager : IEnumerable<BillAmount>
{
    private readonly IDictionary<int, int> bills;

    public IEnumerable<BillAmount> Bills => bills.Select(x => new BillAmount(x.Key, x.Value))
        .ToImmutableList();

    public int Total => bills.Sum(x => x.Key * x.Value);

    public AmountManager()
    {
        bills = new Dictionary<int, int>();
    }

    public AmountManager(IEnumerable<BillAmount> bills)
    {
        this.bills = GroupAmount(bills)
            .ToDictionary(x => x.Denomination, x => x.Quantity);
    }

    public void SetAmount(IEnumerable<BillAmount> bills)
    {
        var groupAmount = GroupAmount(bills).ToList();
        var emptyAmount = this.bills.ExceptBy(groupAmount.Select(x => x.Denomination), x => x.Key)
            .Select(x => new BillAmount(x.Key, 0));

        foreach (var bill in groupAmount.Concat(emptyAmount))
        {
            SetAmount(bill);
        }
    }

    public void SetAmount(BillAmount bill)
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


    public AmountManager Clone()
    {
        return new AmountManager(Bills);
    }

    private IEnumerable<BillAmount> GroupAmount(IEnumerable<BillAmount> bills)
    {
        return bills.GroupBy(x => x.Denomination)
            .Select(x => new BillAmount(x.Key, x.Sum(b => b.Quantity)));
    }

    public IEnumerator<BillAmount> GetEnumerator()
    {
        return Bills.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
