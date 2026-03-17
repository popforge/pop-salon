using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Popsalon.Application;
using Popsalon.Application.EntityViews;
using Popsalon.Infrastructure;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration).WriteTo.Console());

// ── Services ───────────────────────────────────────────────────────────────
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// OData EDM model — une entrée par EntityView généré
static IEdmModel GetEdmModel()
{
    var b = new ODataConventionModelBuilder();
    b.EntitySet<AppointmentView>("appointments");
    b.EntitySet<CustomerView>("customers");
    return b.GetEdmModel();
}

builder.Services
    .AddControllers()
    .AddOData(options => options
        .Select().Filter().OrderBy().Expand().Count().SetMaxTop(200)
        .AddRouteComponents("api/v1", GetEdmModel()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Popsalon API", Version = "v1" });
});

builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// ── Middleware ─────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

// Swagger + Scalar API docs
app.UseSwagger();
app.MapScalarApiReference(options =>
{
    options.Title = "Popsalon API";
    options.Theme = ScalarTheme.Saturn;
});

// Database migration on startup (ignoré en test — InMemory ne supporte pas MigrateAsync)
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider
        .GetRequiredService<Popsalon.Infrastructure.Persistence.ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();

// Exposé pour WebApplicationFactory dans les tests d'intégration
public partial class Program { }
