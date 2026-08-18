using MediatR;

namespace ServiceBooking.Application.BusinessProfiles.Queries.GetBusinessProfileById;

// This query asks for a BusinessProfileDto by its ID
public record GetBusinessProfileByIdQuery(string Id) : IRequest<GetBusinessProfileByIdDto?>;