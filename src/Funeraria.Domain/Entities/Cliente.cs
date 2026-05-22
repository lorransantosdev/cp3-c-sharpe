namespace Funeraria.Domain.Entities;

public abstract class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telefone { get; set; } = null!;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public int FilialId { get; set; }
    public Filial Filial { get; set; } = null!;

    public ICollection<Contratacao> Contratacoes { get; set; } = new List<Contratacao>();

    public abstract string TipoCliente { get; }
}
