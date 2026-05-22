using Funeraria.Api.Dtos;
using Funeraria.Domain.Entities;
using Funeraria.Domain.Messages;
using Funeraria.Infrastructure.Data;
using Funeraria.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeraria.Api.Controllers;

[ApiController]
[Route("api/contratacoes")]
public class ContratacoesController : ControllerBase
{
    private readonly FunerariaDbContext _db;
    private readonly IContratacaoPublisher _publisher;
    private readonly ILogger<ContratacoesController> _logger;

    public ContratacoesController(FunerariaDbContext db, IContratacaoPublisher publisher, ILogger<ContratacoesController> logger)
    {
        _db = db;
        _publisher = publisher;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContratacaoResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Solicitar([FromBody] CriarContratacaoRequest req)
    {
        var cliente = await _db.Clientes.FindAsync(req.ClienteId);
        if (cliente is null)
            return NotFound(new { message = $"Cliente {req.ClienteId} não encontrado." });

        var servico = await _db.Servicos.FindAsync(req.ServicoId);
        if (servico is null)
            return NotFound(new { message = $"Serviço {req.ServicoId} não encontrado." });

        if (!servico.Ativo)
            return BadRequest(new { message = "Serviço inativo." });

        var contratacao = new Contratacao
        {
            ClienteId = req.ClienteId,
            ServicoId = req.ServicoId,
            ValorSolicitado = req.ValorSolicitado,
            PrazoMeses = req.PrazoMeses,
            Status = StatusContratacao.Pendente,
            SolicitadaEm = DateTime.UtcNow
        };
        _db.Contratacoes.Add(contratacao);
        await _db.SaveChangesAsync();

        try
        {
            await _publisher.PublishAsync(new ContratacaoSolicitadaMessage
            {
                ContratacaoId = contratacao.Id,
                ClienteId = contratacao.ClienteId,
                ServicoId = contratacao.ServicoId,
                TipoServico = servico.TipoServico,
                ValorSolicitado = contratacao.ValorSolicitado,
                PrazoMeses = contratacao.PrazoMeses,
                SolicitadaEm = contratacao.SolicitadaEm
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao publicar contratação {Id} na fila", contratacao.Id);
            contratacao.Status = StatusContratacao.Falhou;
            contratacao.Mensagem = "Falha ao enfileirar para processamento.";
            await _db.SaveChangesAsync();
        }

        return AcceptedAtAction(nameof(Buscar), new { id = contratacao.Id }, ToResponse(contratacao, servico.TipoServico));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ContratacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Buscar(int id)
    {
        var c = await _db.Contratacoes
            .Include(x => x.Servico)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (c is null) return NotFound();

        return Ok(ToResponse(c, c.Servico.TipoServico));
    }

    private static ContratacaoResponse ToResponse(Contratacao c, string tipoServico) => new()
    {
        Id = c.Id,
        ClienteId = c.ClienteId,
        ServicoId = c.ServicoId,
        TipoServico = tipoServico,
        ValorSolicitado = c.ValorSolicitado,
        PrazoMeses = c.PrazoMeses,
        Status = c.Status,
        Mensagem = c.Mensagem,
        ScoreCalculado = c.ScoreCalculado,
        TaxaAplicada = c.TaxaAplicada,
        SolicitadaEm = c.SolicitadaEm,
        ProcessadaEm = c.ProcessadaEm
    };
}
