using System.ComponentModel.DataAnnotations;

namespace Funeraria.Api.Dtos;

public class CriarPessoaFisicaRequest
{
    [Required, StringLength(150)]
    public string Nome { get; set; } = null!;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = null!;

    [Required, StringLength(20)]
    public string Telefone { get; set; } = null!;

    [Required]
    public int FilialId { get; set; }

    [Required, StringLength(14, MinimumLength = 11)]
    public string Cpf { get; set; } = null!;

    [Required]
    public DateTime DataNascimento { get; set; }

    [Range(0, double.MaxValue)]
    public decimal RendaMensal { get; set; }
}

public class CriarPessoaJuridicaRequest
{
    [Required, StringLength(150)]
    public string Nome { get; set; } = null!;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = null!;

    [Required, StringLength(20)]
    public string Telefone { get; set; } = null!;

    [Required]
    public int FilialId { get; set; }

    [Required, StringLength(18, MinimumLength = 14)]
    public string Cnpj { get; set; } = null!;

    [Required, StringLength(200)]
    public string RazaoSocial { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public decimal FaturamentoMensal { get; set; }
}

public class ClienteResponse
{
    public int Id { get; set; }
    public string Tipo { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telefone { get; set; } = null!;
    public int FilialId { get; set; }
    public string? Cpf { get; set; }
    public DateTime? DataNascimento { get; set; }
    public decimal? RendaMensal { get; set; }
    public string? Cnpj { get; set; }
    public string? RazaoSocial { get; set; }
    public decimal? FaturamentoMensal { get; set; }
    public DateTime CriadoEm { get; set; }
}
