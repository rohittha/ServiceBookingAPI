using Moq;
using ServiceBooking.Application.Bookings.Commands.CreateBooking;
using ServiceBooking.Application.Common.Interfaces;
using ServiceBooking.Domain.Entities;
using ServiceBooking.Domain.ValueObjects;
using Xunit;

namespace ServiceBooking.UnitTests.Application.Bookings;

public class CreateBookingCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepoMock;
    private readonly Mock<IBusinessProfileRepository> _businessRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly CreateBookingCommandHandler _handler;

    public CreateBookingCommandHandlerTests()
    {
        // 1. Initialize Mocks
        _bookingRepoMock = new Mock<IBookingRepository>();
        _businessRepoMock = new Mock<IBusinessProfileRepository>();
        _uowMock = new Mock<IUnitOfWork>();

        // 2. Inject Mocks into the Handler
        _handler = new CreateBookingCommandHandler(
            _bookingRepoMock.Object,
            _businessRepoMock.Object,
            _uowMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnBookingId_WhenBusinessExistsAndRequestIsValid()
    {
        // Arrange (Setup the world)
        var command = new CreateBookingCommand(
            BusinessId: "biz_123",
            ServiceId: "ser_456",
            EmployeeId: "emp_789",
            BookingDateTime: DateTime.UtcNow.AddDays(1),
            CustomerEmail: "test@client.com");

        // Mock the business repository to return a valid BusinessProfile
        var business = new BusinessProfile("Test Business", "logo.png");
        _businessRepoMock
            .Setup(x => x.GetByIdAsync(command.BusinessId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(business);

        // Act (Perform the action)
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Verify the results)
        Assert.NotNull(result);
        Assert.IsType<string>(result);

        // Verify that AddAsync was called exactly once
        _bookingRepoMock.Verify(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);

        // Verify that SaveChangesAsync was called exactly once
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenBusinessDoesNotExist()
    {
        // Arrange
        var command = new CreateBookingCommand("invalid_id", "ser_1", "emp_1", DateTime.UtcNow.AddDays(1), "email@test.com");

        // Setup the mock to return NULL (Business not found)
        _businessRepoMock
            .Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BusinessProfile?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Business not found.", exception.Message);

        // Verify that the booking was NEVER added or saved because it failed early
        _bookingRepoMock.Verify(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentException_WhenBookingDateIsInPast()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-5);
        var command = new CreateBookingCommand("biz_1", "ser_1", "emp_1", pastDate, "email@test.com");

        // Business exists, but the Domain Logic inside Booking.cs will fail
        _businessRepoMock
            .Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BusinessProfile("Name", null));

        // Act & Assert
        // This confirms that our Application Layer is correctly triggering the Domain Layer's guards
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }
}