# Multi-Tenancy et Communication Inter-Clusters

> **Date** : Mars 2026  
> **Statut** : Spécification de référence

## Stratégie multi-tenant

### Approche choisie : Database-per-Tenant

Chaque tenant possède sa propre base de données PostgreSQL. Cette approche offre :

| Critère | Database-per-Tenant | Schema-per-Tenant | Row-per-Tenant |
|---|---|---|---|
| **Isolation des données** | ✅ Maximale | ✅ Bonne | ⚠️ Logique seulement |
| **Simplicité des requêtes** | ✅ Aucun filtre requis | ⚠️ Préfixe schema | ❌ WHERE tenant_id partout |
| **Conformité RGPD** | ✅ Facile (backup/delete par DB) | ⚠️ Complexe | ❌ Très complexe |
| **Scalabilité** | ⚠️ Coûteux à grande échelle | ✅ Bon compromis | ✅ Le plus économique |
| **Migration de schéma** | ⚠️ Doit tourner par tenant | ⚠️ Doit tourner par schema | ✅ Une seule migration |

**Décision** : Database-per-Tenant pour les clusters métier et TenantManagement.  
Pour LicenseManagement, Row-per-Tenant est acceptable (données peu sensibles, faible volume).

---

## Implémentation avec Finbuckle.MultiTenant

### Configuration du store tenant

```csharp
// Infrastructure/DependencyInjection.cs

builder.Services
    .AddMultiTenant<TenantInfo>()
    
    // Stratégie de résolution : lit le claim "tenant_id" du JWT
    .WithClaimStrategy("tenant_id")
    
    // Fallback : header HTTP X-Tenant-Id (pour les appels inter-clusters)
    .WithHeaderStrategy("X-Tenant-Id")
    
    // Store : les tenants sont chargés depuis TenantManagement
    .WithEFCoreStore<TenantStoreDbContext, TenantInfo>();
```

### Modèle `TenantInfo`

```csharp
// Infrastructure/MultiTenancy/TenantInfo.cs
public class TenantInfo : ITenantInfo
{
    public string? Id { get; set; }               // GUID du tenant
    public string? Identifier { get; set; }       // slug unique (ex: "salon-marie")
    public string? Name { get; set; }             // Nom affiché
    public string? ConnectionString { get; set; } // Connexion DB spécifique à ce tenant
    
    // Extensions métier
    public string? Timezone { get; set; }
    public string[]? EnabledModules { get; set; }
    public DateTime? LicenseExpiresAt { get; set; }
}
```

### DbContext multi-tenant

```csharp
// Infrastructure/Persistence/ApplicationDbContext.cs
public class ApplicationDbContext : DbContext, IMultiTenantDbContext
{
    public ITenantInfo? TenantInfo { get; }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantInfo tenantInfo)
        : base(options)
    {
        TenantInfo = tenantInfo;
    }

    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Finbuckle ajoute automatiquement les query filters tenant
        // si les entités implémentent IHaveQueryFilter
        builder.ConfigureMultiTenant();
    }
}
```

### Factory pour changer de connexion par tenant

```csharp
// Infrastructure/Persistence/TenantDbContextFactory.cs
public class TenantDbContextFactory : IDbContextFactory<ApplicationDbContext>
{
    private readonly IMultiTenantContextAccessor<TenantInfo> _tenantAccessor;
    private readonly DbContextOptions<ApplicationDbContext> _baseOptions;

    public ApplicationDbContext CreateDbContext()
    {
        var tenant = _tenantAccessor.MultiTenantContext?.TenantInfo
            ?? throw new InvalidOperationException("Tenant non résolu.");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>(_baseOptions);
        
        // Surcharge la connection string avec celle du tenant
        optionsBuilder.UseNpgsql(tenant.ConnectionString);
        
        return new ApplicationDbContext(optionsBuilder.Options, tenant);
    }
}
```

### Migration automatique par tenant

```csharp
// Program.cs — migration au démarrage pour chaque tenant connu
public static async Task MigrateAllTenantsAsync(IServiceProvider services)
{
    var tenantStore = services.GetRequiredService<IMultiTenantStore<TenantInfo>>();
    var tenants = await tenantStore.GetAllAsync();

    foreach (var tenant in tenants)
    {
        using var scope = services.CreateScope();
        var accessor = scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor<TenantInfo>>();
        accessor.MultiTenantContext = new MultiTenantContext<TenantInfo> { TenantInfo = tenant };

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
```

---

## Communication inter-clusters

### Diagramme des dépendances

```
BusinessCluster
    → TenantManagement  (synchrone : validation tenant sur chaque requête)
    → LicenseManagement (synchrone : validation licence au login + périodique)

TenantManagement
    → LicenseManagement (synchrone : vérification licence lors du provisioning)

LicenseManagement
    → (aucune dépendance externe — cluster racine)
```

### Appels synchrones via Refit

```csharp
// Shared.Contracts/ITenantManagementClient.cs
[Headers("Content-Type: application/json")]
public interface ITenantManagementClient
{
    [Get("/api/v1/tenants/{tenantId}")]
    Task<TenantDto?> GetTenantAsync(string tenantId, CancellationToken ct = default);

    [Get("/api/v1/tenants/{tenantId}/is-active")]
    Task<bool> IsTenantActiveAsync(string tenantId, CancellationToken ct = default);
}

// Shared.Contracts/ILicenseManagementClient.cs
public interface ILicenseManagementClient
{
    [Get("/api/v1/licenses/validate")]
    Task<LicenseValidationResult> ValidateLicenseAsync(
        [Query] string tenantId,
        [Query] string clusterName,
        CancellationToken ct = default);
}
```

```csharp
// Enregistrement dans Infrastructure/DependencyInjection.cs
services.AddRefitClient<ITenantManagementClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(configuration["TenantManagement:BaseUrl"]!);
        c.DefaultRequestHeaders.Add("X-Internal-Api-Key", configuration["InternalApiKey"]);
    })
    .AddStandardResilienceHandler(); // Polly retry + circuit breaker

services.AddRefitClient<ILicenseManagementClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(configuration["LicenseManagement:BaseUrl"]!);
        c.DefaultRequestHeaders.Add("X-Internal-Api-Key", configuration["InternalApiKey"]);
    })
    .AddStandardResilienceHandler();
```

### Middleware de validation tenant + licence

```csharp
// Api/Middleware/TenantValidationMiddleware.cs
public class TenantValidationMiddleware
{
    private readonly RequestDelegate _next;

    public async Task InvokeAsync(HttpContext context,
        ITenantManagementClient tenantClient,
        IMultiTenantContextAccessor<TenantInfo> tenantAccessor)
    {
        var tenantId = tenantAccessor.MultiTenantContext?.TenantInfo?.Id;

        if (tenantId is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var isActive = await tenantClient.IsTenantActiveAsync(tenantId);
        if (!isActive)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant inactif ou inexistant." });
            return;
        }

        await _next(context);
    }
}
```

### Événements asynchrones via MassTransit + RabbitMQ

```csharp
// Shared.Contracts/Events/TenantCreatedEvent.cs
public record TenantCreatedEvent(
    string TenantId,
    string TenantName,
    string AdminEmail,
    DateTime CreatedAt
);

// Shared.Contracts/Events/LicenseExpiredEvent.cs
public record LicenseExpiredEvent(
    string TenantId,
    string ClusterName,
    DateTime ExpiredAt
);
```

```csharp
// TenantManagement — publie l'événement après création
public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, string>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public async Task<string> Handle(CreateTenantCommand request, CancellationToken ct)
    {
        // ... créer le tenant ...
        
        await _publishEndpoint.Publish(new TenantCreatedEvent(
            TenantId: tenant.Id,
            TenantName: tenant.Name,
            AdminEmail: request.AdminEmail,
            CreatedAt: DateTime.UtcNow
        ), ct);

        return tenant.Id;
    }
}

// BusinessCluster — réagit à la création de tenant
public class TenantCreatedEventConsumer : IConsumer<TenantCreatedEvent>
{
    public async Task Consume(ConsumeContext<TenantCreatedEvent> context)
    {
        // Provision automatique : créer la DB du tenant, appliquer les migrations
        await ProvisionTenantDatabaseAsync(context.Message.TenantId);
    }
}
```

```csharp
// Configuration MassTransit dans Program.cs
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TenantCreatedEventConsumer>();
    x.AddConsumer<LicenseExpiredEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"]);
            h.Password(builder.Configuration["RabbitMQ:Password"]);
        });

        cfg.ConfigureEndpoints(context);
    });
});
```

---

## Sécurité inter-clusters

Les appels HTTP entre clusters ne passent pas par Keycloak — ils utilisent une **clé API interne** partagée via les secrets de déploiement (vault ou variables d'environnement Docker).

```csharp
// Middleware côté serveur pour valider la clé interne
public class InternalApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _expectedApiKey;

    public async Task InvokeAsync(HttpContext context)
    {
        // Endpoints internes préfixés /internal/
        if (context.Request.Path.StartsWithSegments("/internal"))
        {
            var providedKey = context.Request.Headers["X-Internal-Api-Key"];
            if (providedKey != _expectedApiKey)
            {
                context.Response.StatusCode = 401;
                return;
            }
        }

        await _next(context);
    }
}
```

---

## Provisioning d'un nouveau tenant — Flux complet

```
1. Admin se connecte à l'interface TenantManagement
2. Remplit le formulaire : Nom, Admin Email, Cluster(s) assigné(s)
3. POST /api/v1/tenants (TenantManagement API)
4. TenantManagement :
   a. Valide la licence via LicenseManagement
   b. Crée l'enregistrement tenant en base
   c. Génère la connection string unique (ex: PopSalon_Tenant_abc123)
   d. Publie TenantCreatedEvent sur RabbitMQ
5. BusinessCluster reçoit TenantCreatedEvent :
   a. Crée la base de données PostgreSQL du tenant
   b. Applique toutes les migrations EF Core
   c. Crée le compte admin initial
6. L'admin du tenant reçoit un email d'invitation (lien de premier accès)
7. Tenant opérationnel en ~10 secondes
```

---

## Variables d'environnement liées au multi-tenant

```bash
# BusinessCluster
TENANT_MANAGEMENT_BASE_URL=http://tenant-management-api:5001
LICENSE_MANAGEMENT_BASE_URL=http://license-management-api:5002
INTERNAL_API_KEY=<secret>           # Partagé entre tous les clusters

# Connexion DB du store tenant (liste des tenants connus)
TENANT_STORE_CONNECTION_STRING=Server=postgres-tenant;Database=TenantStore;...

# Template de connection string par tenant (le TenantId est substitué)
TENANT_DB_CONNECTION_TEMPLATE=Server=postgres-business;Database=PopSalon_{0};User Id=postgres;Password=...
```

---

## Voir aussi

- [architecture-overview.md](architecture-overview.md) — Rôle de chaque cluster
- [cluster-backend-spec.md](cluster-backend-spec.md) — DbContext et repositories
- [deployment-spec.md](deployment-spec.md) — Configuration Docker des 3 clusters
