namespace ServiceBooking.Application.Bookings.Queries.Common;

public record BookingResponseDto(
    string Id,
    string BusinessId,
    string ServiceId,
    DateTime BookingDateTime,
    string CustomerEmail,
    string? EmployeeId);
