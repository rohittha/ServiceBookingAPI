using ServiceBooking.Application.BusinessProfiles.CommonDto;

namespace ServiceBooking.Application.BusinessProfiles.Queries.GetBusinessProfileById;
public record GetBusinessProfileByIdDto(
    string Id,
    string Name,
    string? LogoUrl,
    List<EmployeeDto> Employees,
    List<ServiceCategoryDto> Categories);
