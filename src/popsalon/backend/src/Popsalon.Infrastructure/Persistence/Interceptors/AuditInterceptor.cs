using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Popsalon.Domain.Entities;

namespace Popsalon.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Intercepteur EF Core : remplit automatiquement CreatedAt et UpdatedAt
/// sur toutes les entités héritant de BusinessEntity.
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyAuditTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        ApplyAuditTimestamps(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private static void ApplyAuditTimestamps(DbContext? context)
    {
        if (context is null) return;
        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Modified && entry.Entity is BusinessEntity<Guid> entity)
                entity.SetUpdatedAt(now);
        }
    }
}
