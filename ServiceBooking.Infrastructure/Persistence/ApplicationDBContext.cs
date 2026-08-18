using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServiceBooking.Application.Common.Interfaces;
using ServiceBooking.Domain.Common;
using ServiceBooking.Domain.Entities;
using ServiceBooking.Domain.ValueObjects;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServiceBooking.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BusinessProfile> BusinessProfiles => Set<BusinessProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Iterate through entities to apply global rules FIRST
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
        {
            // Skip owned types
            if (entityType.IsOwned()) continue;

            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var builder = modelBuilder.Entity(entityType.ClrType);

                // Ignore DomainEvents for Root Entities
                builder.Ignore(nameof(BaseEntity.DomainEvents));

                // Map Id to 'id' for Cosmos DB
                builder.Property(nameof(BaseEntity.Id)).ToJsonProperty("id");
            }
        }

        // 2. Apply specific configurations from files (Fluent API classes)
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 3. Value converters for domain value objects to prevent EF treating them as entities
        var emailConverter = new ValueConverter<Email, string>(
            v => v.Value,
            s => Email.Create(s));

        var durationConverter = new ValueConverter<Duration, int>(
            v => v.TotalMinutes,
            m => Duration.FromMinutes(m));

        // Apply converters at the model metadata level so we don't call modelBuilder.Entity<T>()
        // (calling Entity<T>() can fail when EF already marked T as owned)
        foreach (var et in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var prop in et.GetProperties())
            {
                if (prop.ClrType == typeof(Email))
                {
                    prop.SetValueConverter(emailConverter);
                }

                if (prop.ClrType == typeof(Duration))
                {
                    prop.SetValueConverter(durationConverter);
                }
            }
        }

        // 4. Fix the Discriminator issue
        // IMPORTANT: If BusinessProfile inherits from Profile, 
        // you MUST apply HasNoDiscriminator to the BASE class.
        modelBuilder.Entity<BusinessProfile>().HasNoDiscriminator();

        // If you have a class named 'Profile' that BusinessProfile inherits from, 
        // uncomment the line below:
        // modelBuilder.Entity<Profile>().HasNoDiscriminator();

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}