using System.Collections.Immutable;

namespace ATM.Logic.Model;

public readonly struct BillAmmount
{
    public int Denomination { get; init; }
    public int Quantity { get; init; }

    public int Ammount => Denomination * Quantity;

    public BillAmmount(int denomination, int quantity)
    {
        if (denomination < 0)
            throw new ArgumentOutOfRangeException(nameof(denomination));

        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        Denomination = denomination;
        Quantity = quantity;
    }

    public BillAmmount(int denomination) : this(denomination, 0)
    {
    
    }

}