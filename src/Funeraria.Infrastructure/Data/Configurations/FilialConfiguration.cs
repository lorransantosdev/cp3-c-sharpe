using Funeraria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Funeraria.Infrastructure.Data.Configurations;

public class FilialConfiguration : IEntityTypeConfiguration<Filial>
{
    public void Configure(EntityTypeBuilder<Filial> b)
    {
        b.ToTable("FILIAIS");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("ID").ValueGeneratedOnAdd();
        b.Property(x => x.Numero).HasColumnName("NUMERO").HasMaxLength(10).IsRequired();
        b.Property(x => x.Nome).HasColumnName("NOME").HasMaxLength(120).IsRequired();
        b.Property(x => x.Endereco).HasColumnName("ENDERECO").HasMaxLength(200).IsRequired();
        b.HasIndex(x => x.Numero).IsUnique();
    }
}
