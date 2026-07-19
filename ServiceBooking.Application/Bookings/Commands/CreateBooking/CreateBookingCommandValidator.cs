using FluentValidation;

namespace ServiceBooking.Application.Bookings.Commands.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(v => v.BusinessId)
            .NotEmpty().WithMessage("BusinessId is required.");

        RuleFor(v => v.ServiceId)
            .NotEmpty().WithMessage("ServiceId is required.");

        RuleFor(v => v.CustomerEmail)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(v => v.BookingDateTime)
            .NotEmpty()
            .Must(dateTime => dateTime > DateTime.UtcNow)
            .WithMessage("Booking date must be in the future.");
    }
}