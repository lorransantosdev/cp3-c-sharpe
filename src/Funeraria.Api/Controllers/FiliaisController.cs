using Funeraria.Api.Dtos;
using Funeraria.Domain.Entities;
using Funeraria.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeraria.Api.Controllers;

[ApiController]
[Route("api/filiais")]
public class FiliaisController : ControllerBase
{
    private readonly FunerariaDbContext _db;

    public FiliaisController(FunerariaDbContext db) => _db = db;

    [HttpPost]
    [ProducesResponseType(typeof(FilialResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Criar([FromBody] CriarFilialRequest req)
    {
        if (await _db.Filiais.AnyAsync(a => a.Numero == req.Numero))
            return Conflict(new { message = "Já existe uma filial com este número." });

        var filial = new Filial { Numero = req.Numero, Nome = req.Nome, Endereco = req.Endereco };
        _db.Filiais.Add(filial);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Buscar), new { id = filial.Id }, ToResponse(filial));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FilialResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Buscar(int id)
    {
        var filial = await _db.Filiais.FindAsync(id);
        if (filial is null) return NotFound();
        return Ok(ToResponse(filial));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FilialResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var filiais = await _db.Filiais.AsNoTracking().ToListAsync();
        return Ok(filiais.Select(ToResponse));
    }

    private static FilialResponse ToResponse(Filial f) => new()
    {
        Id = f.Id,
        Numero = f.Numero,
        Nome = f.Nome,
        Endereco = f.Endereco
    };
}
