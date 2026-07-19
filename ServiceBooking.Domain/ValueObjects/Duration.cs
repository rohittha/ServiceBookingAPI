namespace ServiceBooking.Domain.ValueObjects;

public record Duration
{
    public int TotalMinutes { get; init; }

    private Duration(int minutes)
    {
        if (minutes <= 0)
            throw new ArgumentException("Duration must be greater than 0 minutes.");

        if (minutes > 480) // 8 hours cap as an example business rule
            throw new ArgumentException("Duration cannot exceed 8 hours.");

        TotalMinutes = minutes;
    }

    public static Duration FromMinutes(int minutes) => new Duration(minutes);

    public override string ToString() => $"{TotalMinutes} mins";
}


