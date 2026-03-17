using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Popsalon.Application.Common.Interfaces;
using Popsalon.Infrastructure.Persistence;
using Xunit;

namespace Popsalon.Api.IntegrationTests;

/// <summary>
/// Factory de test qui remplace PostgreSQL par une base de données en mémoire.
/// La migration automatique au démarrage est ignorée grâce à EnsureCreated().
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Fournir une connection string factice pour que AddInfrastructureServices
        // puisse s'enregistrer sans lever d'exception — le DbContext sera remplacé ensuite.
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = "Host=localhost;Database=test;Username=test;Password=test"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Supprimer tous les enregistrements liés au DbContext réel (Npgsql)
            var toRemove = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
                         || d.ServiceType == typeof(ApplicationDbContext))
                .ToList();
            foreach (var d in toRemove) services.Remove(d);

            // Remplacer par InMemory
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("PopsalonTestDb_" + Guid.NewGuid()));

            services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());

            // Créer le schéma in-memory
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
