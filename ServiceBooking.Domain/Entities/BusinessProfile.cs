using ServiceBooking.Domain.Common;
using ServiceBooking.Domain.ValueObjects;

namespace ServiceBooking.Domain.Entities;

public class BusinessProfile : BaseEntity, IAggregateRoot
{
    // Use 'protected set' to prevent external code from bypassing business logic
    public string Name { get; private set; } = null!;
    public string? LogoUrl { get; private set; }

    // Use List fields but expose as IReadOnlyCollection
    // This prevents: profile.Employees.Clear() from outside this class
    private readonly List<Employee> _employees = new();
    public virtual IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();

    private readonly List<ServiceCategory> _serviceCategories = new();
    public virtual IReadOnlyCollection<ServiceCategory> ServiceCategories => _serviceCategories.AsReadOnly();

    // Required for EF Core
    private BusinessProfile() { }

    // Constructor ensures the entity is always in a valid state
    public BusinessProfile(string name, string? logoUrl)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        Name = name;
        LogoUrl = logoUrl;
    }
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        Name = name;
    }

    // Business Logic: Add an employee with validation
    public void AddEmployee(string name, string email, string position)
    {
        var emailVo = Email.Create(email); 
        if (_employees.Any(e => e.Email == email))
            throw new InvalidOperationException("Employee email already exists! Please enter a unique value.");

        _employees.Add(new Employee(name, emailVo, position));
    }

    public void AddServiceCategory(string name, string imageUrl)
    {
        // 1. Enforce Invariant: Uniqueness check
        // We use OrdinalIgnoreCase because "Spa" and "spa" are usually considered the same category
        if (_serviceCategories.Any(sc => sc.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"A service category with the name '{name}' already exists.");
        }

        // 2. State Change: Add the new entity
        // We create the entity here so we can keep its constructor 'internal'
        var category = new ServiceCategory(name, imageUrl);
        _serviceCategories.Add(category);
    }

    public void AddServiceToCategory(string categoryId, string name, string description, decimal price, int durationInMinutes)
    {
        var category = _serviceCategories.FirstOrDefault(c => c.Id == categoryId);
        if (category == null) throw new Exception("Category not found.");

        // Convert int to Value Object
        var durationVo = Duration.FromMinutes(durationInMinutes);

        category.AddService(name, description, price, durationVo);
    }
}
