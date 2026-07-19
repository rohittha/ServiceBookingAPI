namespace ServiceBooking.Application.BusinessProfiles.Queries.GetBusinessProfileById;
public record BusinessProfileDto(
    string Id,
    string Name,
    string? LogoUrl,
    List<EmployeeDto> Employees,
    List<ServiceCategoryDto> Categories);
