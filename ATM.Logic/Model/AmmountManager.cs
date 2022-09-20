using System.Collections;
using System.Collections.Immutable;

namespace ATM.Logic.Model;

public class AmmountManager : IEnumerable<BillAmmount>
{
    private readonly IDictionary<int, int> bills;

    public IEnumerable<BillAmmount> Bills => bills.Select(x => new BillAmmount(x.Key, x.Value))
        .ToImmutableList();

    public int Total => bills.Sum(x => x.Key * x.Value);

    public AmmountManager()
    {
        bills = new Dictionary<int, int>();
    }

    public AmmountManager(IEnumerable<BillAmmount> bills)
    {
        this.bills = GroupAmmount(bills)
            .ToDictionary(x => x.Denomination, x => x.Quantity);
    }

    public void SetAmount(IEnumerable<BillAmmount> bills)
    {
        var groupAmount = GroupAmmount(bills).ToList();
        var emptyAmount = this.bills.ExceptBy(groupAmount.Select(x => x.Denomination), x => x.Key)
            .Select(x => new BillAmmount(x.Key, 0));

        foreach (var bill in groupAmount.Concat(emptyAmount))
        {
            SetAmount(bill);
        }
    }

    public void SetAmount(BillAmmount bill)
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


    public AmmountManager Clone()
    {
        return new AmmountManager(Bills);
    }

    private IEnumerable<BillAmmount> GroupAmmount(IEnumerable<BillAmmount> bills)
    {
        return bills.GroupBy(x => x.Denomination)
            .Select(x => new BillAmmount(x.Key, x.Sum(b => b.Quantity)));
    }

    public IEnumerator<BillAmmount> GetEnumerator()
    {
        return Bills.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
