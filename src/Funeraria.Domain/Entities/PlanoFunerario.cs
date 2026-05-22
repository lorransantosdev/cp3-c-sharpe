namespace Funeraria.Domain.Entities;

public class PlanoFunerario : Servico
{
    public decimal MensalidadeBase { get; set; }
    public int CarenciaMaximaMeses { get; set; }
    public decimal CoberturaMaxima { get; set; }

    public override string TipoServico => "PLANO_FUNERARIO";
}
