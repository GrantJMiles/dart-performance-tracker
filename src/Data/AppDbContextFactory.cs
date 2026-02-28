using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DartPerformanceTracker.Data;

/// <summary>
/// Used exclusively by EF Core design-time tooling (dotnet ef migrations add / database update).
/// Not used at runtime — the running app gets its connection string from the
/// AZURE_SQL_CONNECTION_STRING environment variable configured in Program.cs.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=DartPerformanceTracker;User ID=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;");
        return new AppDbContext(optionsBuilder.Options);
    }
}
