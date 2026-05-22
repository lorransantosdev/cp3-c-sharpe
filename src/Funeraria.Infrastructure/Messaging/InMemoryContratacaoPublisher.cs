using Funeraria.Domain.Messages;
using Microsoft.Extensions.Logging;

namespace Funeraria.Infrastructure.Messaging;

public class InMemoryContratacaoPublisher : IContratacaoPublisher
{
    private readonly InMemoryContratacaoQueue _queue;
    private readonly ILogger<InMemoryContratacaoPublisher> _logger;

    public InMemoryContratacaoPublisher(InMemoryContratacaoQueue queue, ILogger<InMemoryContratacaoPublisher> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    public async Task PublishAsync(ContratacaoSolicitadaMessage mensagem, CancellationToken ct = default)
    {
        await _queue.Writer.WriteAsync(mensagem, ct);
        _logger.LogInformation("Contratação {Id} enfileirada para processamento assíncrono", mensagem.ContratacaoId);
    }
}
