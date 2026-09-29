using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBooking.Application.Common.Interfaces;
using ServiceBooking.Application.Common.Models;
using ServiceBooking.Domain.Events;

namespace ServiceBooking.Application.Bookings.EventHandlers;

public class BookingCreatedEventHandler : INotificationHandler<BookingCreatedEvent>
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<BookingCreatedEventHandler> _logger;
    private const string QueueName = "booking-created-queue";

    public BookingCreatedEventHandler(
        IMessageBus messageBus,
        ILogger<BookingCreatedEventHandler> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"---> [Gate A3] Handler reached! BookingId: {notification.Booking.Id}, CustomerEmail: {notification.Booking.CustomerEmail.Value}");
        var booking = notification.Booking;

        _logger.LogInformation("Domain Event: Forwarding BookingCreatedEvent for Booking ID {BookingId} to Azure Service Bus.", booking.Id);

        var integrationMessage = new BookingCreatedIntegrationEvent(
            BookingId: booking.Id,
            BusinessId: booking.BusinessId,
            ServiceId: booking.ServiceId,
            EmployeeId: booking.EmployeeId,
            BookingDateTime: booking.BookingDateTime,
            CustomerEmail: booking.CustomerEmail.Value
        );

        await _messageBus.PublishAsync(integrationMessage, QueueName, cancellationToken);
    }
}