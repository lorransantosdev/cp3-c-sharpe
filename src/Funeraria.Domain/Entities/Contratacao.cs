namespace Funeraria.Domain.Entities;

public class Contratacao
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int ServicoId { get; set; }
    public Servico Servico { get; set; } = null!;

    public decimal ValorSolicitado { get; set; }
    public int? PrazoMeses { get; set; }

    public StatusContratacao Status { get; set; } = StatusContratacao.Pendente;
    public string? Mensagem { get; set; }
    public int? ScoreCalculado { get; set; }
    public decimal? TaxaAplicada { get; set; }

    public DateTime SolicitadaEm { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessadaEm { get; set; }
}
