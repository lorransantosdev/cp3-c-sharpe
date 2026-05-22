using System.ComponentModel.DataAnnotations;
using Funeraria.Domain.Entities;

namespace Funeraria.Api.Dtos;

public class CriarContratacaoRequest
{
    [Required]
    public int ClienteId { get; set; }

    [Required]
    public int ServicoId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ValorSolicitado { get; set; }

    public int? PrazoMeses { get; set; }
}

public class ContratacaoResponse
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int ServicoId { get; set; }
    public string TipoServico { get; set; } = null!;
    public decimal ValorSolicitado { get; set; }
    public int? PrazoMeses { get; set; }
    public StatusContratacao Status { get; set; }
    public string StatusDescricao => Status.ToString();
    public string? Mensagem { get; set; }
    public int? ScoreCalculado { get; set; }
    public decimal? TaxaAplicada { get; set; }
    public DateTime SolicitadaEm { get; set; }
    public DateTime? ProcessadaEm { get; set; }
}
