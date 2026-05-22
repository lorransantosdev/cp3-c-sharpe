namespace Funeraria.Domain.Messages;

public class ContratacaoSolicitadaMessage
{
    public int ContratacaoId { get; set; }
    public int ClienteId { get; set; }
    public int ServicoId { get; set; }
    public string TipoServico { get; set; } = null!;
    public decimal ValorSolicitado { get; set; }
    public int? PrazoMeses { get; set; }
    public DateTime SolicitadaEm { get; set; }
}
