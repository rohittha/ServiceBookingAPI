using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceBooking.Domain.Entities;
using ServiceBooking.Domain.ValueObjects;

namespace ServiceBooking.Infrastructure.Persistence.Configurations;

public class BusinessProfileConfiguration : IEntityTypeConfiguration<BusinessProfile>
{
    public void Configure(EntityTypeBuilder<BusinessProfile> builder)
    {
        builder.ToContainer("BusinessProfiles"); // Ensure container name is explicit
        builder.HasPartitionKey(bp => bp.Id);     // Highly recommended for Cosmos
        builder.Property(bp => bp.Id).ToJsonProperty("id");

        builder.OwnsMany(bp => bp.Employees, eb =>
        {
            eb.Property(e => e.Id).ToJsonProperty("id");
            eb.Property(e => e.Name).ToJsonProperty("Name");
            eb.Property(e => e.Position).ToJsonProperty("Position");
            eb.Property(e => e.IsActive).ToJsonProperty("IsActive");

            // REMOVE THIS:
            // eb.OwnsOne(e => e.Email, ...) 

            // Instead, just map the property name if it differs, 
            // but your global converter handles the logic.
            eb.Property(e => e.Email).ToJsonProperty("Email");

            eb.Property<List<string>>("_serviceIds")
                .HasField("_serviceIds")
                .ToJsonProperty("ServiceIds");
        });

        builder.OwnsMany(bp => bp.ServiceCategories, scb =>
        {
            scb.Property(c => c.Id).ToJsonProperty("id");
            scb.Property(c => c.Name).ToJsonProperty("Name");
            scb.Property(c => c.ImageUrl).ToJsonProperty("ImageUrl");

            scb.OwnsMany(c => c.Services, sb =>
            {
                sb.Property(s => s.Id).ToJsonProperty("id");
                sb.Property(s => s.Name).ToJsonProperty("Name");
                sb.Property(s => s.Description).ToJsonProperty("Description");
                sb.Property(s => s.Price).ToJsonProperty("Price");

                // REMOVE THIS:
                // sb.OwnsOne(s => s.Duration, ...)

                // Map it as a property so the ValueConverter can handle the int <-> Duration logic
                sb.Property(s => s.Duration).ToJsonProperty("Duration");
            });
        });
    }
}

