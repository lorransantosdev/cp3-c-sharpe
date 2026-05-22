using Funeraria.Domain.Entities;
using Funeraria.Domain.Messages;
using Funeraria.Domain.Services;
using Funeraria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Funeraria.Infrastructure.Messaging;

public class ContratacaoBackgroundConsumer : BackgroundService
{
    private readonly InMemoryContratacaoQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ContratacaoBackgroundConsumer> _logger;

    public ContratacaoBackgroundConsumer(
        InMemoryContratacaoQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<ContratacaoBackgroundConsumer> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consumer de contratações iniciado (fila in-memory)");

        await foreach (var msg in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessarAsync(msg, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro processando contratação {Id}", msg.ContratacaoId);
            }
        }
    }

    private async Task ProcessarAsync(ContratacaoSolicitadaMessage msg, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FunerariaDbContext>();

        var contratacao = await db.Contratacoes.FirstOrDefaultAsync(c => c.Id == msg.ContratacaoId, ct);
        if (contratacao is null)
        {
            _logger.LogWarning("Contratação {Id} não encontrada", msg.ContratacaoId);
            return;
        }

        contratacao.Status = StatusContratacao.EmAnalise;
        await db.SaveChangesAsync(ct);

        await Task.Delay(500, ct);

        var cliente = await db.Clientes.FirstOrDefaultAsync(c => c.Id == msg.ClienteId, ct);
        var servico = await db.Servicos.FirstOrDefaultAsync(p => p.Id == msg.ServicoId, ct);

        if (cliente is null || servico is null)
        {
            contratacao.Status = StatusContratacao.Falhou;
            contratacao.Mensagem = "Cliente ou serviço não encontrado durante processamento.";
            contratacao.ProcessadaEm = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return;
        }

        var resultado = AnaliseContratacaoService.Analisar(cliente, servico, contratacao.ValorSolicitado, contratacao.PrazoMeses);
        contratacao.Status = resultado.Status;
        contratacao.Mensagem = resultado.Mensagem;
        contratacao.ScoreCalculado = resultado.Score;
        contratacao.TaxaAplicada = resultado.TaxaAplicada;
        contratacao.ProcessadaEm = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        _logger.LogInformation("Contratação {Id} processada — status {Status} score {Score}",
            contratacao.Id, contratacao.Status, contratacao.ScoreCalculado);
    }
}
