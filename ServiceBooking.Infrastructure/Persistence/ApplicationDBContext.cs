using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServiceBooking.Application.Common.Interfaces;
using ServiceBooking.Domain.Common;
using ServiceBooking.Domain.Entities;
using ServiceBooking.Domain.ValueObjects;

namespace ServiceBooking.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    // --- 1. FIELD & CONSTRUCTOR ---
    private readonly IMediator? _mediator;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMediator? mediator = null) : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BusinessProfile> BusinessProfiles => Set<BusinessProfile>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Email>()
            .HaveConversion<EmailConverter>();

        configurationBuilder.Properties<Duration>()
            .HaveConversion<DurationConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var entityTypes = modelBuilder.Model.GetEntityTypes().ToList();

        foreach (var entityType in entityTypes)
        {
            if (entityType.IsOwned()) continue;

            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType, b =>
                {
                    b.Ignore(nameof(BaseEntity.DomainEvents));
                    b.Property(nameof(BaseEntity.Id)).ToJsonProperty("id");
                });
            }
        }

        modelBuilder.Entity<Booking>().HasNoDiscriminator();
        modelBuilder.Entity<BusinessProfile>().HasNoDiscriminator();

        base.OnModelCreating(modelBuilder);
    }

    // --- 2. SAVE CHANGES WITH DOMAIN EVENT DISPATCH ---
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEventsAsync(cancellationToken);
        return result;
    }

    // --- 3. DISPATCH METHOD CASTING TO (object) ---
    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        if (_mediator == null) return;

        var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}

public class EmailConverter : ValueConverter<Email, string>
{
    public EmailConverter() : base(v => v.Value, s => Email.Create(s)) { }
}

public class DurationConverter : ValueConverter<Duration, int>
{
    public DurationConverter() : base(v => v.TotalMinutes, m => Duration.FromMinutes(m)) { }
}