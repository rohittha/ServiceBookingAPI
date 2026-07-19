using ServiceBooking.Domain.Entities;
using ServiceBooking.Domain.ValueObjects;
using Xunit;

namespace ServiceBooking.Domain.UnitTests.Entities;

public class BookingTests
{
    [Fact]
    public void Constructor_ShouldThrowException_WhenDateIsInPast()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var email = Email.Create("test@example.com");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Booking("biz_123", "serv_123", "emp_123", pastDate, email));
    }
}
