using ServiceBooking.Domain.ValueObjects;

namespace ServiceBooking.Domain.Entities;
public class Employee
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string Position { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    private readonly List<string> _serviceIds = new();
    public IReadOnlyCollection<string> ServiceIds => _serviceIds.AsReadOnly();

    // EF Core
    private Employee() { }

    public Employee(string name, Email email, string position)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Email = email;
        Position = position;
    }

    public void AssignService(string serviceId)
    {
        if (!_serviceIds.Contains(serviceId))
            _serviceIds.Add(serviceId);
    }
}