using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace ATM.Logic.Model;

public readonly struct BillAmount
{
    public int Denomination { get; init; }
    public int Quantity { get; init; }

    public int Amount => Denomination * Quantity;

    public BillAmount(int denomination, int quantity)
    {
        if (denomination < 0)
            throw new ArgumentOutOfRangeException(nameof(denomination));

        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        Denomination = denomination;
        Quantity = quantity;
    }

    public BillAmount(int denomination) : this(denomination, 0)
    {
    
    }

}