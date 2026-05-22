using Funeraria.Domain.Messages;

namespace Funeraria.Infrastructure.Messaging;

public interface IContratacaoPublisher
{
    Task PublishAsync(ContratacaoSolicitadaMessage mensagem, CancellationToken ct = default);
}
