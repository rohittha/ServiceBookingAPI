using ServiceBooking.Domain.Entities;

namespace ServiceBooking.Application.Common.Interfaces;

public interface IBusinessProfileRepository
{
    Task<BusinessProfile?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task UpdateAsync(BusinessProfile businessProfile, CancellationToken cancellationToken = default);
}