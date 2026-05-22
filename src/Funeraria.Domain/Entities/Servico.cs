namespace Funeraria.Domain.Entities;

public abstract class Servico
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public bool Ativo { get; set; } = true;

    public abstract string TipoServico { get; }
}
