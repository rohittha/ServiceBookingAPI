using MediatR;
using ServiceBooking.Application.BusinessProfiles.CommonDto;
using ServiceBooking.Application.BusinessProfiles.Queries.GetBusinessProfileById;
using ServiceBooking.Application.Common.Interfaces;

namespace ServiceBooking.Application.BusinessProfiles.Queries.GetBusinessProfileById;

public class GetBusinessProfileByIdQueryHandler : IRequestHandler<GetBusinessProfileByIdQuery, GetBusinessProfileByIdDto?>
{
    private readonly IBusinessProfileRepository _repository;

    public GetBusinessProfileByIdQueryHandler(IBusinessProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetBusinessProfileByIdDto?> Handle(GetBusinessProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var business = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (business == null) return null;

        // Manual Mapping (Entity -> DTO)
        // Note: In a larger app, you might use AutoMapper or Mapperly here.
        return new GetBusinessProfileByIdDto(
            business.Id,
            business.Name,
            business.LogoUrl,
            business.Employees.Select(e => new EmployeeDto(e.Id, e.Name, e.Position, e.Email)).ToList(),
            business.ServiceCategories.Select(c => new ServiceCategoryDto(
                c.Id,
                c.Name,
                c.ImageUrl,
                c.Services.Select(s => new ServiceDto(s.Id, s.Name, s.Price, s.Duration.TotalMinutes)).ToList()
            )).ToList()
        );
    }
}