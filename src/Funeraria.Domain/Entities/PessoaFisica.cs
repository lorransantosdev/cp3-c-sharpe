namespace Funeraria.Domain.Entities;

public class PessoaFisica : Cliente
{
    public string Cpf { get; set; } = null!;
    public DateTime DataNascimento { get; set; }
    public decimal RendaMensal { get; set; }

    public override string TipoCliente => "PF";
}
