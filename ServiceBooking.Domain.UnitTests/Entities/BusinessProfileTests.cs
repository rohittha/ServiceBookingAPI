using ServiceBooking.Domain.Entities;
using Xunit;

namespace ServiceBooking.Domain.UnitTests.Entities;

public class BusinessProfileTests
{
    [Fact]
    public void AddEmployee_WithUniqueEmail_ShouldSucceed()
    {
        // Arrange
        var profile = new BusinessProfile("Barber Shop", "logo.png");

        // Act
        profile.AddEmployee("John Doe", "john@example.com", "Senior Barber");

        // Assert
        Assert.Single(profile.Employees);
        Assert.Equal("John Doe", profile.Employees.First().Name);
    }

    [Fact]
    public void AddEmployee_WithDuplicateEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var profile = new BusinessProfile("Barber Shop", null);
        var email = "duplicate@example.com";
        profile.AddEmployee("John One", email, "Barber");

        // Act & Assert
        // This confirms our "Domain Guarding" logic is working
        var exception = Assert.Throws<InvalidOperationException>(() =>
            profile.AddEmployee("John Two", email, "Stylist"));

        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public void AddServiceCategory_WithDuplicateName_ShouldThrowException()
    {
        // Arrange
        var profile = new BusinessProfile("Spa Center", null);
        profile.AddServiceCategory("Massage", "img.png");

        // Act & Assert (Testing case-insensitivity rule)
        Assert.Throws<InvalidOperationException>(() =>
            profile.AddServiceCategory("massage", "other.png"));
    }
}