using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Popsalon.Application.Common.Interfaces;
using Popsalon.Domain.Interfaces;
using Popsalon.Infrastructure.Persistence;
using Popsalon.Infrastructure.Persistence.Interceptors;
using Popsalon.Infrastructure.Persistence.Repositories;

namespace Popsalon.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<AuditInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
        });

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        // Repositories
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }
}
