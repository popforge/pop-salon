// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using Microsoft.EntityFrameworkCore;
using Popsalon.Domain.Entities;
using Popsalon.Domain.Interfaces;
using Popsalon.Infrastructure.Persistence;

namespace Popsalon.Infrastructure.Persistence.Repositories;

public class CustomerRepository(ApplicationDbContext db) : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Customers.FindAsync([id], ct);

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken ct = default)
        => await db.Customers.AnyAsync(
            c => c.Email == email.ToLowerInvariant() && (excludeId == null || c.Id != excludeId),
            ct);

    public async Task AddAsync(Customer entity, CancellationToken ct = default)
    {
        await db.Customers.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Customer entity, CancellationToken ct = default)
    {
        db.Customers.Update(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Customer entity, CancellationToken ct = default)
    {
        db.Customers.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}
