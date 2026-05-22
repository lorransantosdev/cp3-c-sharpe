using System.Net;
using System.Net.Http.Json;
using Funeraria.Api.Dtos;
using Funeraria.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Funeraria.Tests;

public class FiliaisTests : IClassFixture<FunerariaApiFactory>
{
    private readonly FunerariaApiFactory _factory;
    public FiliaisTests(FunerariaApiFactory factory) => _factory = factory;

    [Fact]
    public async Task CriarFilial_Sucesso_RetornaCreated()
    {
        var client = _factory.CreateClient();
        var resp = await client.PostAsJsonAsync("/api/filiais", new CriarFilialRequest
        {
            Numero = "9001",
            Nome = "Filial Teste",
            Endereco = "Av. Brasil, 1"
        });
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task BuscarFilial_Inexistente_Retorna404()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/api/filiais/99999");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
