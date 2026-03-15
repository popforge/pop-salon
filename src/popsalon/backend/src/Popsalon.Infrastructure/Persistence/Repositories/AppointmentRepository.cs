// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using Microsoft.EntityFrameworkCore;
using Popsalon.Domain.Entities;
using Popsalon.Domain.Interfaces;
using Popsalon.Infrastructure.Persistence;

namespace Popsalon.Infrastructure.Persistence.Repositories;

public class AppointmentRepository(ApplicationDbContext db) : IAppointmentRepository
{
    public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Appointments.FindAsync([id], ct);

    public async Task AddAsync(Appointment entity, CancellationToken ct = default)
    {
        await db.Appointments.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Appointment entity, CancellationToken ct = default)
    {
        db.Appointments.Update(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Appointment entity, CancellationToken ct = default)
    {
        db.Appointments.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}
