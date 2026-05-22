using Funeraria.Api.Dtos;
using Funeraria.Domain.Entities;
using Funeraria.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeraria.Api.Controllers;

[ApiController]
[Route("api/servicos")]
public class ServicosController : ControllerBase
{
    private readonly FunerariaDbContext _db;

    public ServicosController(FunerariaDbContext db) => _db = db;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ServicoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var servicos = await _db.Servicos.AsNoTracking().ToListAsync();
        return Ok(servicos.Select(s => new ServicoResponse
        {
            Id = s.Id,
            Tipo = s.TipoServico,
            Nome = s.Nome,
            Descricao = s.Descricao
        }));
    }

    [HttpPost("plano-funerario")]
    [ProducesResponseType(typeof(ServicoResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CriarPlanoFunerario([FromBody] CriarPlanoFunerarioRequest req)
    {
        var p = new PlanoFunerario
        {
            Nome = req.Nome,
            Descricao = req.Descricao,
            MensalidadeBase = req.MensalidadeBase,
            CarenciaMaximaMeses = req.CarenciaMaximaMeses,
            CoberturaMaxima = req.CoberturaMaxima
        };
        _db.PlanosFunerarios.Add(p);
        await _db.SaveChangesAsync();
        return Created($"/api/servicos/{p.Id}", new ServicoResponse { Id = p.Id, Tipo = p.TipoServico, Nome = p.Nome, Descricao = p.Descricao });
    }

    [HttpPost("servico-assistencial")]
    [ProducesResponseType(typeof(ServicoResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CriarServicoAssistencial([FromBody] CriarServicoAssistencialRequest req)
    {
        var s = new ServicoAssistencial
        {
            Nome = req.Nome,
            Descricao = req.Descricao,
            TaxaPacoteBasico = req.TaxaPacoteBasico,
            TaxaPacotePremium = req.TaxaPacotePremium,
            TaxaDeslocamento = req.TaxaDeslocamento
        };
        _db.ServicosAssistenciais.Add(s);
        await _db.SaveChangesAsync();
        return Created($"/api/servicos/{s.Id}", new ServicoResponse { Id = s.Id, Tipo = s.TipoServico, Nome = s.Nome, Descricao = s.Descricao });
    }
}
