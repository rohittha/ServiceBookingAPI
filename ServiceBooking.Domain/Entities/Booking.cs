using ServiceBooking.Domain.Common;
using ServiceBooking.Domain.Events;
using ServiceBooking.Domain.ValueObjects;

namespace ServiceBooking.Domain.Entities;

public class Booking : BaseEntity, IAggregateRoot
{
    public string BusinessId { get; private set; } = null!;
    public string ServiceId { get; private set; } = null!;
    public string? EmployeeId { get; private set; } // Already nullable, so it's fine
    public DateTime BookingDateTime { get; private set; }
    public Email CustomerEmail { get; private set; } = null!; // Changed

    // 1. EF Core Constructor
    // We use 'null!' or leave them unitialized here because EF Core 
    // will set them via reflection during hydration from the DB.
    private Booking() { } // EF Core

    public Booking(string businessId, string serviceId, string? employeeId, DateTime dateTime, Email customerEmail)
    {
        if (string.IsNullOrWhiteSpace(businessId)) throw new ArgumentException("BusinessId is required");
        if (string.IsNullOrWhiteSpace(serviceId)) throw new ArgumentException("ServiceId is required");
        if (dateTime < DateTime.UtcNow) throw new ArgumentException("Booking cannot be in the past");

        BusinessId = businessId;
        ServiceId = serviceId;
        EmployeeId = employeeId;
        BookingDateTime = dateTime;
        CustomerEmail = customerEmail;

        AddDomainEvent(new BookingCreatedEvent(this));
    }
}
