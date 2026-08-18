using MediatR;
using ServiceBooking.Application.BusinessProfiles.CommonDto;

namespace ServiceBooking.Application.BusinessProfiles.Commands.UpdateBusinessProfile;

// IRequest<string> means this command will return the ID of the created booking
public record UpdateBusinessProfileCommand(
    string Id,
    string Name,
    string? LogoUrl,
    List<EmployeeDto> Employees,
    List<ServiceCategoryDto> Categories) : IRequest<string>;