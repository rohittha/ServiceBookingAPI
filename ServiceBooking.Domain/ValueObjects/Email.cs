using System.Text.RegularExpressions;

namespace ServiceBooking.Domain.ValueObjects;

public record Email
{
    // Pre-compile the regex for performance
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public string Value { get; init; }

    private Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email is required.");

        var trimmedEmail = value.Trim();

        if (!EmailRegex.IsMatch(trimmedEmail))
            throw new ArgumentException("Invalid email format.");

        Value = trimmedEmail.ToLowerInvariant();
    }

    public static Email Create(string value) => new Email(value);

    public static implicit operator string(Email email) => email.Value;

    public override string ToString() => Value;
}
