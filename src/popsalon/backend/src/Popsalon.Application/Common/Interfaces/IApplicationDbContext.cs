using Microsoft.EntityFrameworkCore;
using Popsalon.Domain.Entities;

namespace Popsalon.Application.Common.Interfaces;

/// <summary>
/// Abstraction du DbContext exposée à la couche Application.
/// Seules les opérations de lecture (AsNoTracking) et le SaveChangesAsync sont exposés.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Appointment> Appointments { get; }
    DbSet<Customer> Customers { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
