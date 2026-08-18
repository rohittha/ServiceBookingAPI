
using Microsoft.EntityFrameworkCore;
using ServiceBooking.Application.Common.Interfaces;
using ServiceBooking.Domain.Entities;

namespace ServiceBooking.Infrastructure.Persistence.Repositories;

public class BusinessProfileRepository : IBusinessProfileRepository
{
    private readonly ApplicationDbContext _context;

    public BusinessProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    async Task<BusinessProfile?> IBusinessProfileRepository.GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        // For Cosmos DB owned collections are embedded in the same document.
        // Avoid explicit Include/ThenInclude here — let EF materialize owned collections
        // from the document to prevent Cosmos provider materialization errors.
        return await _context.BusinessProfiles
            .WithPartitionKey(id) // If Id is your partition key, adding this improves performance
            .SingleOrDefaultAsync(bp => bp.Id == id, cancellationToken);
    }

    Task IBusinessProfileRepository.UpdateAsync(BusinessProfile businessProfile, CancellationToken cancellationToken)
    {
        _context.BusinessProfiles.Update(businessProfile);
        return Task.CompletedTask;
    }
}