using MediatR;

namespace ServiceBooking.Application.Bookings.Commands.CreateBooking;

// IRequest<string> means this command will return the ID of the created booking
public record CreateBookingCommand(
    string BusinessId,
    string ServiceId,
    string? EmployeeId,
    DateTime BookingDateTime,
    string CustomerEmail) : IRequest<string>;