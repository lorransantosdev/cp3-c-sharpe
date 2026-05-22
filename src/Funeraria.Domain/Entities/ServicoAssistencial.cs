namespace Funeraria.Domain.Entities;

public class ServicoAssistencial : Servico
{
    public decimal TaxaPacoteBasico { get; set; }
    public decimal TaxaPacotePremium { get; set; }
    public decimal TaxaDeslocamento { get; set; }

    public override string TipoServico => "SERVICO_ASSISTENCIAL";
}
