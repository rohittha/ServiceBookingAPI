namespace ServiceBooking.Application.BusinessProfiles.CommonDto;
public record ServiceCategoryDto(
    string Id,
    string Name,
    string ImageUrl,
    List<ServiceDto> Services);
