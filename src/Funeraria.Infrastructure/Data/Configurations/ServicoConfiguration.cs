using Funeraria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Funeraria.Infrastructure.Data.Configurations;

public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
{
    public void Configure(EntityTypeBuilder<Servico> b)
    {
        b.ToTable("SERVICOS");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("ID").ValueGeneratedOnAdd();
        b.Property(x => x.Nome).HasColumnName("NOME").HasMaxLength(120).IsRequired();
        b.Property(x => x.Descricao).HasColumnName("DESCRICAO").HasMaxLength(400).IsRequired();
        b.Property(x => x.Ativo)
            .HasColumnName("ATIVO")
            .HasColumnType("NUMBER(1)")
            .HasConversion<int>()
            .IsRequired();

        b.HasDiscriminator<string>("TIPO_SERVICO")
            .HasValue<ServicoAssistencial>("SERVICO_ASSISTENCIAL")
            .HasValue<Jazigo>("JAZIGO")
            .HasValue<PlanoFunerario>("PLANO_FUNERARIO");
    }
}

public class ServicoAssistencialConfiguration : IEntityTypeConfiguration<ServicoAssistencial>
{
    public void Configure(EntityTypeBuilder<ServicoAssistencial> b)
    {
        b.Property(x => x.TaxaPacoteBasico).HasColumnName("TAXA_PACOTE_BASICO").HasPrecision(6, 4);
        b.Property(x => x.TaxaPacotePremium).HasColumnName("TAXA_PACOTE_PREMIUM").HasPrecision(6, 4);
        b.Property(x => x.TaxaDeslocamento).HasColumnName("TAXA_DESLOCAMENTO").HasPrecision(18, 2);
    }
}

public class JazigoConfiguration : IEntityTypeConfiguration<Jazigo>
{
    public void Configure(EntityTypeBuilder<Jazigo> b)
    {
        b.Property(x => x.CnpjCemiterio).HasColumnName("CNPJ_CEMITERIO").HasMaxLength(18);
        b.Property(x => x.ValorPerpetuidade).HasColumnName("VALOR_PERPETUIDADE").HasPrecision(18, 2);
    }
}

public class PlanoFunerarioConfiguration : IEntityTypeConfiguration<PlanoFunerario>
{
    public void Configure(EntityTypeBuilder<PlanoFunerario> b)
    {
        b.Property(x => x.MensalidadeBase).HasColumnName("MENSALIDADE_BASE").HasPrecision(6, 4);
        b.Property(x => x.CarenciaMaximaMeses).HasColumnName("CARENCIA_MAX_MESES");
        b.Property(x => x.CoberturaMaxima).HasColumnName("COBERTURA_MAXIMA").HasPrecision(18, 2);
    }
}
