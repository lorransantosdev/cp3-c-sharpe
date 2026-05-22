using Funeraria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Funeraria.Infrastructure.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> b)
    {
        b.ToTable("CLIENTES");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("ID").ValueGeneratedOnAdd();
        b.Property(x => x.Nome).HasColumnName("NOME").HasMaxLength(150).IsRequired();
        b.Property(x => x.Email).HasColumnName("EMAIL").HasMaxLength(150).IsRequired();
        b.Property(x => x.Telefone).HasColumnName("TELEFONE").HasMaxLength(20).IsRequired();
        b.Property(x => x.CriadoEm).HasColumnName("CRIADO_EM").IsRequired();
        b.Property(x => x.FilialId).HasColumnName("FILIAL_ID").IsRequired();

        b.HasOne(x => x.Filial)
            .WithMany(a => a.Clientes)
            .HasForeignKey(x => x.FilialId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasDiscriminator<string>("TIPO_CLIENTE")
            .HasValue<PessoaFisica>("PF")
            .HasValue<PessoaJuridica>("PJ");
    }
}

public class PessoaFisicaConfiguration : IEntityTypeConfiguration<PessoaFisica>
{
    public void Configure(EntityTypeBuilder<PessoaFisica> b)
    {
        b.Property(x => x.Cpf).HasColumnName("CPF").HasMaxLength(14);
        b.Property(x => x.DataNascimento).HasColumnName("DATA_NASCIMENTO");
        b.Property(x => x.RendaMensal).HasColumnName("RENDA_MENSAL").HasPrecision(18, 2);
        b.HasIndex(x => x.Cpf).IsUnique().HasFilter(null);
    }
}

public class PessoaJuridicaConfiguration : IEntityTypeConfiguration<PessoaJuridica>
{
    public void Configure(EntityTypeBuilder<PessoaJuridica> b)
    {
        b.Property(x => x.Cnpj).HasColumnName("CNPJ").HasMaxLength(18);
        b.Property(x => x.RazaoSocial).HasColumnName("RAZAO_SOCIAL").HasMaxLength(200);
        b.Property(x => x.FaturamentoMensal).HasColumnName("FATURAMENTO_MENSAL").HasPrecision(18, 2);
        b.HasIndex(x => x.Cnpj).IsUnique().HasFilter(null);
    }
}
