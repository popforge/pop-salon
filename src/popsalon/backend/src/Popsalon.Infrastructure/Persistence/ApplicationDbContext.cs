using Microsoft.EntityFrameworkCore;
using Popsalon.Application.Common.Interfaces;
using Popsalon.Domain.Entities;
using Popsalon.Infrastructure.Persistence.Interceptors;
using System.Reflection;

namespace Popsalon.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    AuditInterceptor auditInterceptor)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(auditInterceptor);
    }
}
