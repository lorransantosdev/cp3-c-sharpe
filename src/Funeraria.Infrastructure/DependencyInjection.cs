using Funeraria.Infrastructure.Data;
using Funeraria.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;

namespace Funeraria.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFunerariaInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Oracle");

        services.AddDbContext<FunerariaDbContext>(opts =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                opts.UseOracle(connectionString, o => o.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19));
            }
        });

        services.AddSingleton<InMemoryContratacaoQueue>();
        services.AddSingleton<IContratacaoPublisher, InMemoryContratacaoPublisher>();
        services.AddHostedService<ContratacaoBackgroundConsumer>();

        return services;
    }
}
