using Funeraria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Funeraria.Infrastructure.Data.Configurations;

public class ContratacaoConfiguration : IEntityTypeConfiguration<Contratacao>
{
    public void Configure(EntityTypeBuilder<Contratacao> b)
    {
        b.ToTable("CONTRATACOES");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("ID").ValueGeneratedOnAdd();
        b.Property(x => x.ClienteId).HasColumnName("CLIENTE_ID").IsRequired();
        b.Property(x => x.ServicoId).HasColumnName("SERVICO_ID").IsRequired();
        b.Property(x => x.ValorSolicitado).HasColumnName("VALOR_SOLICITADO").HasPrecision(18, 2);
        b.Property(x => x.PrazoMeses).HasColumnName("PRAZO_MESES");
        b.Property(x => x.Status).HasColumnName("STATUS").HasConversion<int>().IsRequired();
        b.Property(x => x.Mensagem).HasColumnName("MENSAGEM").HasMaxLength(500);
        b.Property(x => x.ScoreCalculado).HasColumnName("SCORE_CALCULADO");
        b.Property(x => x.TaxaAplicada).HasColumnName("TAXA_APLICADA").HasPrecision(6, 4);
        b.Property(x => x.SolicitadaEm).HasColumnName("SOLICITADA_EM").IsRequired();
        b.Property(x => x.ProcessadaEm).HasColumnName("PROCESSADA_EM");

        b.HasOne(x => x.Cliente)
            .WithMany(c => c.Contratacoes)
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Servico)
            .WithMany()
            .HasForeignKey(x => x.ServicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
