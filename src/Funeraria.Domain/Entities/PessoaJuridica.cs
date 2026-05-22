namespace Funeraria.Domain.Entities;

public class PessoaJuridica : Cliente
{
    public string Cnpj { get; set; } = null!;
    public string RazaoSocial { get; set; } = null!;
    public decimal FaturamentoMensal { get; set; }

    public override string TipoCliente => "PJ";
}
