using Funeraria.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Funeraria.Infrastructure.Data;

public class FunerariaDbContext : DbContext
{
    public FunerariaDbContext(DbContextOptions<FunerariaDbContext> options) : base(options) { }

    public DbSet<Filial> Filiais => Set<Filial>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<PessoaFisica> PessoasFisicas => Set<PessoaFisica>();
    public DbSet<PessoaJuridica> PessoasJuridicas => Set<PessoaJuridica>();
    public DbSet<Servico> Servicos => Set<Servico>();
    public DbSet<ServicoAssistencial> ServicosAssistenciais => Set<ServicoAssistencial>();
    public DbSet<Jazigo> Jazigos => Set<Jazigo>();
    public DbSet<PlanoFunerario> PlanosFunerarios => Set<PlanoFunerario>();
    public DbSet<Contratacao> Contratacoes => Set<Contratacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FunerariaDbContext).Assembly);
    }
}
