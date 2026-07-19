using ServiceBooking.Application.Bookings.Commands.CreateBooking;
using Xunit;

namespace ServiceBooking.UnitTests.Application.Bookings;

public class CreateBookingValidatorTests
{
    private readonly CreateBookingCommandValidator _validator = new();

    [Fact]
    public void Validator_ShouldHaveError_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new CreateBookingCommand("biz_1", "ser_1", "emp_1", DateTime.UtcNow.AddDays(1), "not-an-email");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CustomerEmail");
    }
}