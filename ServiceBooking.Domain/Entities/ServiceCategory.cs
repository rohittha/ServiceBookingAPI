using ServiceBooking.Domain.ValueObjects;

namespace ServiceBooking.Domain.Entities;

public class ServiceCategory
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string Name { get; private set; } = null!;
    public string ImageUrl { get; private set; } = null!;

    private readonly List<Service> _services = new();
    public virtual IReadOnlyCollection<Service> Services => _services.AsReadOnly();

    // EF Core Constructor
    private ServiceCategory() { }

    public ServiceCategory(string name, string imageUrl)
    {
        Id = Guid.NewGuid().ToString();
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
        if (string.IsNullOrWhiteSpace(imageUrl)) throw new ArgumentException("ImageUrl cannot be empty.");
        Name = name;
        ImageUrl = imageUrl;
    }

    public void AddService(string name, string description, decimal price, Duration duration)
    {
        if (_services.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Service with this name already exists.");

        _services.Add(new Service(name, description, price, duration));
    }
}