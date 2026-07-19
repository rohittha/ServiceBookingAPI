using ServiceBooking.Domain.Common;
using ServiceBooking.Domain.Entities;

namespace ServiceBooking.Domain.Events;
public class BookingCreatedEvent : BaseEvent
{
    public Booking Booking { get; }
    public BookingCreatedEvent(Booking booking) => Booking = booking;
}

