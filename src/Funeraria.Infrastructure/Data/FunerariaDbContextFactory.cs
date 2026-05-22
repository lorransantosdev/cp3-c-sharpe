using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;

namespace Funeraria.Infrastructure.Data;

public class FunerariaDbContextFactory : IDesignTimeDbContextFactory<FunerariaDbContext>
{
    public FunerariaDbContext CreateDbContext(string[] args)
    {
        var apiPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Funeraria.Api"));
        if (!File.Exists(Path.Combine(apiPath, "appsettings.json")))
            apiPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "src", "Funeraria.Api"));

        var config = new ConfigurationBuilder()
            .SetBasePath(apiPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config.GetConnectionString("Oracle")
            ?? throw new InvalidOperationException("ConnectionStrings:Oracle não configurada.");

        var options = new DbContextOptionsBuilder<FunerariaDbContext>()
            .UseOracle(connectionString, o => o.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19))
            .Options;

        return new FunerariaDbContext(options);
    }
}
