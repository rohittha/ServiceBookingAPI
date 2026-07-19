namespace ServiceBooking.Application.Common.Interfaces;

public interface IUnitOfWork
{
    // Returns the number of state entries written to the database
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}