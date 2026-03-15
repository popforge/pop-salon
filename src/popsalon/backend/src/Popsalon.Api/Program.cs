using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Popsalon.Application;
using Popsalon.Application.EntityViews;
using Popsalon.Infrastructure;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration).WriteTo.Console());

    // ── Services ───────────────────────────────────────────────────────────────
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // OData EDM model — une entrée par EntityView généré
    static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<AppointmentView>("appointments");
        builder.EntitySet<CustomerView>("customers");
        return builder.GetEdmModel();
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

    builder.Services.AddCors(options =>
        options.AddDefaultPolicy(policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

    // ── Middleware ─────────────────────────────────────────────────────────────
    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Swagger + Scalar API docs
    app.UseSwagger();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Popsalon API";
        options.Theme = ScalarTheme.Saturn;
    });

    // Database migration on startup
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider
            .GetRequiredService<Popsalon.Infrastructure.Persistence.ApplicationDbContext>();
        await db.Database.MigrateAsync();
    }

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application démarrée avec une erreur fatale.");
}
finally
{
    Log.CloseAndFlush();
}
