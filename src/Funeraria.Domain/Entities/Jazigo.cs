namespace Funeraria.Domain.Entities;

public class Jazigo : Servico
{
    public string CnpjCemiterio { get; set; } = null!;
    public decimal ValorPerpetuidade { get; set; }

    public override string TipoServico => "JAZIGO";
}
