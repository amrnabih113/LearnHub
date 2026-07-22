using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LearnHub.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor(ICurrentUserService user, TimeProvider dateTime) : SaveChangesInterceptor
{
    private readonly ICurrentUserService _user = user;
    private readonly TimeProvider _dateTime = dateTime;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                var utcNow = _dateTime.GetUtcNow();

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = _user.UserId;
                    entry.Entity.CreatedAtUtc = utcNow;
                }

                entry.Entity.UpdatedBy = _user.UserId;
                entry.Entity.UpdatedAtUtc = utcNow;

                foreach (var ownedEntry in entry.References)
                {
                    if (ownedEntry.TargetEntry is { Entity: AuditableEntity ownedEntity } && ownedEntry.TargetEntry.State is EntityState.Added or EntityState.Modified)
                    {
                        if (ownedEntry.TargetEntry.State == EntityState.Added)
                        {
                            ownedEntity.CreatedBy = _user.UserId;
                            ownedEntity.CreatedAtUtc = utcNow;
                        }

                        ownedEntity.UpdatedBy = _user.UserId;
                        ownedEntity.UpdatedAtUtc = utcNow;
                    }
                }
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry?.Metadata.IsOwned() == true &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}