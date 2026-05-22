using Funeraria.Api.Dtos;
using Funeraria.Domain.Entities;
using Funeraria.Domain.Validators;
using Funeraria.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeraria.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly FunerariaDbContext _db;

    public ClientesController(FunerariaDbContext db) => _db = db;

    [HttpPost("pf")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CriarPessoaFisica([FromBody] CriarPessoaFisicaRequest req)
    {
        var cpf = DocumentoValidator.NormalizarCpf(req.Cpf);
        if (!DocumentoValidator.IsCpfValido(cpf))
            return BadRequest(new { message = "CPF inválido." });

        var filial = await _db.Filiais.FindAsync(req.FilialId);
        if (filial is null)
            return NotFound(new { message = $"Filial {req.FilialId} não encontrada." });

        if (await _db.PessoasFisicas.AnyAsync(p => p.Cpf == cpf))
            return Conflict(new { message = "Já existe um cliente PF com este CPF." });

        var pf = new PessoaFisica
        {
            Nome = req.Nome,
            Email = req.Email,
            Telefone = req.Telefone,
            FilialId = req.FilialId,
            Cpf = cpf!,
            DataNascimento = req.DataNascimento,
            RendaMensal = req.RendaMensal
        };

        _db.PessoasFisicas.Add(pf);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Buscar), new { id = pf.Id }, ToResponse(pf));
    }

    [HttpPost("pj")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CriarPessoaJuridica([FromBody] CriarPessoaJuridicaRequest req)
    {
        var cnpj = DocumentoValidator.NormalizarCnpj(req.Cnpj);
        if (!DocumentoValidator.IsCnpjValido(cnpj))
            return BadRequest(new { message = "CNPJ inválido." });

        var filial = await _db.Filiais.FindAsync(req.FilialId);
        if (filial is null)
            return NotFound(new { message = $"Filial {req.FilialId} não encontrada." });

        if (await _db.PessoasJuridicas.AnyAsync(p => p.Cnpj == cnpj))
            return Conflict(new { message = "Já existe um cliente PJ com este CNPJ." });

        var pj = new PessoaJuridica
        {
            Nome = req.Nome,
            Email = req.Email,
            Telefone = req.Telefone,
            FilialId = req.FilialId,
            Cnpj = cnpj!,
            RazaoSocial = req.RazaoSocial,
            FaturamentoMensal = req.FaturamentoMensal
        };

        _db.PessoasJuridicas.Add(pj);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Buscar), new { id = pj.Id }, ToResponse(pj));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Buscar(int id)
    {
        var cliente = await _db.Clientes.FindAsync(id);
        if (cliente is null) return NotFound();
        return Ok(ToResponse(cliente));
    }

    private static ClienteResponse ToResponse(Cliente c) => c switch
    {
        PessoaFisica pf => new ClienteResponse
        {
            Id = pf.Id,
            Tipo = "PF",
            Nome = pf.Nome,
            Email = pf.Email,
            Telefone = pf.Telefone,
            FilialId = pf.FilialId,
            Cpf = pf.Cpf,
            DataNascimento = pf.DataNascimento,
            RendaMensal = pf.RendaMensal,
            CriadoEm = pf.CriadoEm
        },
        PessoaJuridica pj => new ClienteResponse
        {
            Id = pj.Id,
            Tipo = "PJ",
            Nome = pj.Nome,
            Email = pj.Email,
            Telefone = pj.Telefone,
            FilialId = pj.FilialId,
            Cnpj = pj.Cnpj,
            RazaoSocial = pj.RazaoSocial,
            FaturamentoMensal = pj.FaturamentoMensal,
            CriadoEm = pj.CriadoEm
        },
        _ => throw new InvalidOperationException("Tipo de cliente desconhecido")
    };
}
