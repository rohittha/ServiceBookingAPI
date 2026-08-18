using Microsoft.EntityFrameworkCore;
using ServiceBooking.Application.Common.Interfaces;
using ServiceBooking.Domain.Entities;

namespace ServiceBooking.Infrastructure.Persistence.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.Bookings.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        await _context.Bookings.AddAsync(booking, cancellationToken);
    }
}