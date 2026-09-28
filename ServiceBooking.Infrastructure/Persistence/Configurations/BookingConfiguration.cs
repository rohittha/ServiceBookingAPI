using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceBooking.Domain.Entities;

namespace ServiceBooking.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        // 1. Container Name
        builder.ToContainer("Bookings");

        // 2. Keys
        builder.HasKey(b => b.Id);
        builder.HasPartitionKey(b => b.BusinessId);

        // 3. Required Fields
        builder.Property(b => b.CustomerEmail).IsRequired();
        builder.Property(b => b.ServiceId).IsRequired();
        builder.Property(b => b.BookingDateTime).IsRequired();
    }
}