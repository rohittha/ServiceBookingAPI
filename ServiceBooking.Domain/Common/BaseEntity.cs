using MediatR;

namespace ServiceBooking.Domain.Common; 

public abstract class BaseEntity
{
    public string Id { get; protected set; } = Guid.NewGuid().ToString();

    // Domain Events allow us to trigger actions (like sending an email) 
    // without coupling the Booking logic to the Email logic.
    private readonly List<BaseEvent> _domainEvents = new();
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

public abstract class BaseEvent : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}