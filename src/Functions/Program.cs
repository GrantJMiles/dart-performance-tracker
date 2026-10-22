using DartPerformanceTracker.Data;
using DartPerformanceTracker.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Local/sandbox development can run on SQLite (DATABASE_PROVIDER=Sqlite) so no SQL Server is needed.
var useSqlite = string.Equals(builder.Configuration["DATABASE_PROVIDER"], "Sqlite", StringComparison.OrdinalIgnoreCase);

if (useSqlite)
{
    var sqliteConnectionString = builder.Configuration["SQLITE_CONNECTION_STRING"] ?? "Data Source=dev.db";
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(sqliteConnectionString));
}
else
{
    var connectionString = builder.Configuration["AZURE_SQL_CONNECTION_STRING"]
        ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
        ?? throw new InvalidOperationException("No SQL connection string configured. Set AZURE_SQL_CONNECTION_STRING or ConnectionStrings:DefaultConnection.");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
}

builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ISeasonService, SeasonService>();
builder.Services.AddScoped<IMatchTypeService, MatchTypeService>();
builder.Services.AddScoped<IGameNightService, GameNightService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

// Apply migrations on startup (SQLite dev databases are created from the model; the migrations target SQL Server)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (useSqlite)
    {
        db.Database.EnsureCreated();

        if (string.Equals(builder.Configuration["SEED_DEV_DATA"], "true", StringComparison.OrdinalIgnoreCase))
        {
            DevDataSeeder.Seed(db);
        }
    }
    else
    {
        db.Database.Migrate();
    }
}

app.Run();
