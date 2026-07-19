using ServiceBooking.Domain.Common;
using ServiceBooking.Domain.ValueObjects;

namespace ServiceBooking.Domain.Entities;
public class Employee : BaseEntity
{
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!; // Changed
    public string Position { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    private readonly List<string> _serviceIds = new();
    public IReadOnlyCollection<string> ServiceIds => _serviceIds.AsReadOnly();

    // EF Core Constructor
    private Employee() { }
    internal Employee(string name, Email email, string position)
    {
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
