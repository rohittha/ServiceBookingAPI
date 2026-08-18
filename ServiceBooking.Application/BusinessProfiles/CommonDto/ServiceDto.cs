namespace ServiceBooking.Application.BusinessProfiles.CommonDto;

public record ServiceDto(
    string Id,
    string Name,
    decimal Price,
    int DurationMinutes);
