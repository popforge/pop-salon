# Cluster Backend — Spécification technique

> **Date** : Mars 2026  
> **Statut** : Spécification de référence  
> **S'applique à** : LicenseManagement, TenantManagement, et tout cluster métier

## Structure de projet par cluster

Chaque cluster backend est une solution .NET indépendante suivant la **Clean Architecture** strictement.

```
MyCluster/backend/
├── MyCluster.sln
├── src/
│   ├── MyCluster.Domain/
│   ├── MyCluster.Application/
│   ├── MyCluster.Infrastructure/
│   └── MyCluster.Api/
├── tests/
│   ├── MyCluster.Domain.Tests/
│   └── MyCluster.Application.Tests/
└── metadata/                         ← Source de vérité YAML pour la génération
    ├── cluster.yml
    └── entities/
        └── *.yml
```

---

## Couche Domain (`MyCluster.Domain`)

**Responsabilité** : Logique métier pure, entités, règles. Aucune dépendance vers l'extérieur.

### Structure interne

```
MyCluster.Domain/
├── Entities/
│   ├── BusinessEntity.cs          ← Classe de base abstraite
│   └── (entités générées + manuelles)
├── ValueObjects/
├── Enums/
├── Rules/
│   ├── ISavingRule.cs             ← Interface avant persistance
│   ├── ISavedRule.cs              ← Interface après persistance
│   └── Implementations/           ← Une classe par règle métier
├── Events/
│   └── IDomainEvent.cs
└── Interfaces/
    └── Repositories/
        └── I{Entity}Repository.cs  ← Interfaces repository (une par entité)
```

### Classe de base des entités

```csharp
// BusinessEntity.cs
public abstract class BusinessEntity<TId>
{
    public TId Id { get; protected set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    internal void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
}
```

### Pattern des règles de domaine

```csharp
// ISavingRule.cs — validé AVANT la persistance
public interface ISavingRule<TEntity> where TEntity : class
{
    Task ValidateAsync(TEntity entity, CancellationToken ct = default);
}

// ISavedRule.cs — exécuté APRÈS la persistance
public interface ISavedRule<TEntity> where TEntity : class
{
    Task ExecuteAsync(TEntity entity, CancellationToken ct = default);
}

// Exemple de règle concrète
public class AppointmentDateMustBeInFutureRule : ISavingRule<Appointment>
{
    public Task ValidateAsync(Appointment entity, CancellationToken ct = default)
    {
        if (entity.Date <= DateTime.UtcNow)
            throw new DomainValidationException("La date doit être dans le futur.");
        return Task.CompletedTask;
    }
}
```

---

## Couche Application (`MyCluster.Application`)

**Responsabilité** : Orchestration, CQRS, DTOs, vues en lecture, interfaces des services externes.

### Structure interne

```
MyCluster.Application/
├── Common/
│   ├── Interfaces/
│   │   ├── IApplicationDbContext.cs
│   │   └── ICurrentUserService.cs
│   ├── Behaviors/                    ← MediatR pipeline behaviors
│   │   ├── ValidationBehavior.cs
│   │   ├── LoggingBehavior.cs
│   │   └── TransactionBehavior.cs
│   └── Exceptions/
│       ├── NotFoundException.cs
│       └── ValidationException.cs
├── EntityViews/                      ← CQRS Read Models (générés)
│   ├── I{Entity}View.cs
│   └── {Entity}View.cs
├── Features/
│   └── {Entity}/
│       ├── Queries/
│       │   ├── GetAll/
│       │   │   ├── GetAll{Entity}Query.cs
│       │   │   └── GetAll{Entity}QueryHandler.cs
│       │   └── GetById/
│       │       ├── Get{Entity}ByIdQuery.cs
│       │       └── Get{Entity}ByIdQueryHandler.cs
│       └── Commands/
│           ├── Create/
│           │   ├── Create{Entity}Command.cs
│           │   ├── Create{Entity}CommandHandler.cs
│           │   └── Create{Entity}CommandValidator.cs
│           ├── Update/
│           └── Delete/
├── Models/
│   ├── {Entity}CreateModel.cs        ← DTOs API (générés)
│   └── {Entity}UpdateModel.cs
└── DependencyInjection.cs            ← Enregistrement DI de la couche Application
```

### Exemple : Query handler pour une View

```csharp
// GetAllAppointmentsQuery.cs
public record GetAllAppointmentsQuery(
    string? Filter,
    string? OrderBy,
    int Skip = 0,
    int Top = 50
) : IRequest<PagedResult<AppointmentView>>;

// GetAllAppointmentsQueryHandler.cs
public class GetAllAppointmentsQueryHandler
    : IRequestHandler<GetAllAppointmentsQuery, PagedResult<AppointmentView>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAllAppointmentsQueryHandler(IApplicationDbContext dbContext)
        => _dbContext = dbContext;

    public async Task<PagedResult<AppointmentView>> Handle(
        GetAllAppointmentsQuery request,
        CancellationToken ct)
    {
        var query = _dbContext.Appointments
            .AsNoTracking()
            .Select(a => new AppointmentView
            {
                Id = a.Id,
                Date = a.Date,
                CustomerFullName = a.Customer.FirstName + " " + a.Customer.LastName
            });

        // Applique OData filter/orderby via ODataLinqExtensions
        query = query.ApplyODataQuery(request.Filter, request.OrderBy);

        var total = await query.CountAsync(ct);
        var items = await query.Skip(request.Skip).Take(request.Top).ToListAsync(ct);

        return new PagedResult<AppointmentView>(items, total);
    }
}
```

### MediatR Pipeline Behaviors

```csharp
// ValidationBehavior.cs — exécute FluentValidation avant chaque Command
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
```

---

## Couche Infrastructure (`MyCluster.Infrastructure`)

**Responsabilité** : EF Core, repositories, migrations, services externes.

### Structure interne

```
MyCluster.Infrastructure/
├── Persistence/
│   ├── ApplicationDbContext.cs       ← Étend MultiTenantDbContext
│   ├── Configurations/               ← IEntityTypeConfiguration<T> par entité
│   │   └── {Entity}Configuration.cs
│   ├── Repositories/                 ← Implémentations des interfaces du Domain
│   │   └── {Entity}Repository.cs
│   ├── Interceptors/
│   │   ├── AuditInterceptor.cs       ← Remplit CreatedAt/UpdatedAt automatiquement
│   │   └── DomainEventInterceptor.cs ← Dispatch les domain events après SaveChanges
│   └── Migrations/
├── Identity/
│   └── CurrentUserService.cs
├── ExternalServices/
│   └── (clients HTTP Refit inter-clusters)
└── DependencyInjection.cs
```

### DbContext multi-tenant

```csharp
// ApplicationDbContext.cs
public class ApplicationDbContext
    : DbContext, IApplicationDbContext, IMultiTenantDbContext
{
    public ITenantInfo TenantInfo { get; }

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

        // Finbuckle : filtre automatique WHERE tenant_id = @tenantId
        builder.ConfigureMultiTenant();
    }
}
```

### Repository pattern

```csharp
// IRepository.cs — interface générique dans Domain
public interface IRepository<TEntity, TId> where TEntity : BusinessEntity<TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);
}

// AppointmentRepository.cs — implémentation dans Infrastructure
public class AppointmentRepository : IAppointmentRepository
{
    private readonly ApplicationDbContext _context;

    public AppointmentRepository(ApplicationDbContext context)
        => _context = context;

    public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    // ...
}
```

---

## Couche API (`MyCluster.Api`)

**Responsabilité** : Exposition HTTP, OData, middleware, documentation.

### Structure interne

```
MyCluster.Api/
├── Program.cs
├── Controllers/
│   ├── {Entity}Controller.cs         ← OData endpoints (générés)
│   └── UiController.cs               ← Manifest UI (menus, permissions)
├── Endpoints/                        ← Minimal API pour les méthodes custom
│   └── {Feature}Endpoints.cs
├── Middleware/
│   ├── TenantMiddleware.cs           ← Résout le tenant depuis le JWT
│   └── ExceptionHandlingMiddleware.cs
├── Filters/
│   └── ValidationFilter.cs
└── appsettings.json
```

### Program.cs — configuration type

```csharp
var builder = WebApplication.CreateBuilder(args);

// Couches Clean Architecture
builder.Services
    .AddDomainServices()
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

// Multi-tenant (Finbuckle)
builder.Services.AddMultiTenant<TenantInfo>()
    .WithClaimStrategy("tenant_id")          // lit le tenantId depuis le JWT
    .WithEFCoreStore<TenantDbContext, TenantInfo>();

// OData
builder.Services.AddControllers()
    .AddOData(options => options
        .Select().Filter().OrderBy().Expand().Count().SetMaxTop(200)
        .AddRouteComponents("api/v1", GetEdmModel()));

// Auth (Keycloak OIDC)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
    });

// Background jobs
builder.Services.AddHangfire(config => config.UsePostgreSqlStorage(...));
builder.Services.AddHangfireServer();

// Scalar API docs
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMultiTenant();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference();

app.Run();
```

### Controller OData type (généré)

```csharp
// AppointmentController.cs — généré par le CLI
[Route("api/v1/appointments")]
[Authorize]
public class AppointmentController : ODataController
{
    private readonly ISender _mediator;

    public AppointmentController(ISender mediator) => _mediator = mediator;

    [HttpGet]
    [EnableQuery(MaxTop = 200, AllowedQueryOptions = AllowedQueryOptions.All)]
    public async Task<IActionResult> GetAll(ODataQueryOptions<AppointmentView> options)
    {
        var result = await _mediator.Send(new GetAllAppointmentsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetAppointmentByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AppointmentCreateModel model)
    {
        var id = await _mediator.Send(new CreateAppointmentCommand(model));
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AppointmentUpdateModel model)
    {
        await _mediator.Send(new UpdateAppointmentCommand(id, model));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteAppointmentCommand(id));
        return NoContent();
    }
}
```

### Endpoint UiController — Manifest UI au login

```csharp
// UiController.cs
[Route("api/v1/ui")]
[Authorize]
public class UiController : ControllerBase
{
    [HttpPost("on-authenticated")]
    public async Task<IActionResult> OnAuthenticated([FromServices] IUiManifestBuilder builder)
    {
        var manifest = await builder.BuildAsync(User);
        // Retourne : profil user, rôles, permissions, liste des vues, menu items
        return Ok(manifest);
    }
}
```

---

## Convention de nommage

| Élément | Convention | Exemple |
|---|---|---|
| Entités | PascalCase | `Appointment` |
| Interfaces | `I` + PascalCase | `IAppointmentRepository` |
| Commands | `{Verb}{Entity}Command` | `CreateAppointmentCommand` |
| Queries | `Get{Entity(s)}{Criteria}Query` | `GetAppointmentByIdQuery` |
| Handlers | `{Command/Query}Handler` | `CreateAppointmentCommandHandler` |
| Views (read models) | `{Entity}View` | `AppointmentView` |
| Controllers | `{Entity}Controller` | `AppointmentController` |
| Validators | `{Command}Validator` | `CreateAppointmentCommandValidator` |

---

## Voir aussi

- [tech-stack.md](tech-stack.md) — Détail des packages NuGet utilisés
- [code-generation-spec.md](code-generation-spec.md) — Ce qui est généré automatiquement
- [multi-tenancy-spec.md](multi-tenancy-spec.md) — Configuration Finbuckle et isolation des données
