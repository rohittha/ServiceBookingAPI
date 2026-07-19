using ServiceBooking.Domain.Common;
using ServiceBooking.Domain.ValueObjects;

namespace ServiceBooking.Domain.Entities;

public class Service : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    public Duration Duration { get; private set; } = null!;

    // EF Core Constructor
    private Service() { }
    // Internal constructor: Only ServiceCategory or BusinessProfile should create this
    internal Service(string name, string description, decimal price, Duration duration)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
        if (price < 0) throw new ArgumentException("Price cannot be negative.");

        Name = name;
        Description = description;
        Price = price;
        Duration = duration;
    }

    // Example of domain logic: Updating price
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentException("Price cannot be negative.");
        Price = newPrice;
    }
}
