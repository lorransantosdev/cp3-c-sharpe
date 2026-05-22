using System.Threading.Channels;
using Funeraria.Domain.Messages;

namespace Funeraria.Infrastructure.Messaging;

public class InMemoryContratacaoQueue
{
    private readonly Channel<ContratacaoSolicitadaMessage> _channel =
        Channel.CreateUnbounded<ContratacaoSolicitadaMessage>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

    public ChannelWriter<ContratacaoSolicitadaMessage> Writer => _channel.Writer;
    public ChannelReader<ContratacaoSolicitadaMessage> Reader => _channel.Reader;
}
