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