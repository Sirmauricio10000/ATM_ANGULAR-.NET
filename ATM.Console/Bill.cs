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
    {
    
    }

}
