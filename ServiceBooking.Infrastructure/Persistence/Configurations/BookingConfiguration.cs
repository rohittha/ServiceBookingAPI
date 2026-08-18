using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceBooking.Domain.Entities;
using ServiceBooking.Domain.ValueObjects;

namespace ServiceBooking.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        // 1. Cosmos DB Container Settings
        builder.ToContainer("Bookings"); // Name of the container in Cosmos
        builder.HasNoDiscriminator();    // Optional: if only storing one type in this container

        // 2. Partition Key (Crucial for performance)
        builder.HasPartitionKey(b => b.BusinessId);

        // 3. Primary Key
        builder.HasKey(b => b.Id);

        // 4. Value Object Conversions (Same as SQL version)
        builder.Property(b => b.CustomerEmail)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value));

        // 5. Handling the BaseEntity Id
        builder.Property(b => b.Id).ToJsonProperty("id"); // Cosmos expects lowercase 'id'
    }
}