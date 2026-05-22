using Funeraria.Domain.Messages;
using Funeraria.Infrastructure.Data;
using Funeraria.Infrastructure.Messaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Funeraria.Tests.Helpers;

public class FunerariaApiFactory : WebApplicationFactory<Program>
{
    public FakeContratacaoPublisher Publisher { get; } = new();
    public string DbName { get; } = "FunerariaTestes_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            var ctxDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<FunerariaDbContext>));
            if (ctxDescriptor is not null) services.Remove(ctxDescriptor);

            services.AddDbContext<FunerariaDbContext>(opts => opts.UseInMemoryDatabase(DbName));

            var pubDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IContratacaoPublisher));
            if (pubDescriptor is not null) services.Remove(pubDescriptor);

            services.AddSingleton<IContratacaoPublisher>(_ => Publisher);
        });
    }
}

public class FakeContratacaoPublisher : IContratacaoPublisher
{
    public List<ContratacaoSolicitadaMessage> Mensagens { get; } = new();

    public Task PublishAsync(ContratacaoSolicitadaMessage mensagem, CancellationToken ct = default)
    {
        Mensagens.Add(mensagem);
        return Task.CompletedTask;
    }
}
