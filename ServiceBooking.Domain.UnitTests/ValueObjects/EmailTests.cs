using ServiceBooking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ServiceBooking.Domain.UnitTests.ValueObjects;
public class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldReturnEmailObject()
    {
        string inputEmail = "abc@xyz.com";

        Email outputEmail = Email.Create(inputEmail);

        Assert.Equal(inputEmail, outputEmail);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("")]
    public void Create_WithInvalidEmail_ShouldThrowException(string invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
    }
}
