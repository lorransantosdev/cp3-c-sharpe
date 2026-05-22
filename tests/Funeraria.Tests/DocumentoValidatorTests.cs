using Funeraria.Domain.Validators;
using FluentAssertions;
using Xunit;

namespace Funeraria.Tests;

public class DocumentoValidatorTests
{
    [Theory]
    [InlineData("529.982.247-25", true)]
    [InlineData("111.444.777-35", true)]
    [InlineData("111.111.111-11", false)]
    [InlineData("123", false)]
    [InlineData(null, false)]
    public void IsCpfValido(string? cpf, bool esperado)
    {
        DocumentoValidator.IsCpfValido(cpf).Should().Be(esperado);
    }

    [Theory]
    [InlineData("11.222.333/0001-81", true)]
    [InlineData("45.997.418/0001-53", true)]
    [InlineData("00.000.000/0000-00", false)]
    [InlineData("123", false)]
    public void IsCnpjValido(string? cnpj, bool esperado)
    {
        DocumentoValidator.IsCnpjValido(cnpj).Should().Be(esperado);
    }
}
