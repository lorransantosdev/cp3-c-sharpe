using System.Net;
using System.Net.Http.Json;
using Funeraria.Api.Dtos;
using Funeraria.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Funeraria.Tests;

public class ClientesPfTests : IClassFixture<FunerariaApiFactory>
{
    private readonly FunerariaApiFactory _factory;

    public ClientesPfTests(FunerariaApiFactory factory) => _factory = factory;

    private async Task<int> CriarFilialAsync(HttpClient client, string numero = "0001")
    {
        var resp = await client.PostAsJsonAsync("/api/filiais", new CriarFilialRequest
        {
            Numero = numero,
            Nome = "Filial Centro",
            Endereco = "Av. Paulista, 1000"
        });
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var f = await resp.Content.ReadFromJsonAsync<FilialResponse>();
        return f!.Id;
    }

    [Fact]
    public async Task CriarPf_Sucesso_RetornaCreated()
    {
        var client = _factory.CreateClient();
        var filialId = await CriarFilialAsync(client, "0010");

        var resp = await client.PostAsJsonAsync("/api/clientes/pf", new CriarPessoaFisicaRequest
        {
            Nome = "João da Silva",
            Email = "joao@example.com",
            Telefone = "11999990000",
            FilialId = filialId,
            Cpf = TestData.CpfValido1,
            DataNascimento = new DateTime(1990, 1, 15),
            RendaMensal = 8000m
        });

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await resp.Content.ReadFromJsonAsync<ClienteResponse>();
        body!.Tipo.Should().Be("PF");
        body.Cpf.Should().Be("52998224725");
    }

    [Fact]
    public async Task CriarPf_CpfDuplicado_RetornaConflict()
    {
        var client = _factory.CreateClient();
        var filialId = await CriarFilialAsync(client, "0011");

        var req = new CriarPessoaFisicaRequest
        {
            Nome = "Maria",
            Email = "maria@example.com",
            Telefone = "11888880000",
            FilialId = filialId,
            Cpf = TestData.CpfValido2,
            DataNascimento = new DateTime(1985, 5, 5),
            RendaMensal = 5000m
        };

        var first = await client.PostAsJsonAsync("/api/clientes/pf", req);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await client.PostAsJsonAsync("/api/clientes/pf", req);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CriarPf_FilialInexistente_Retorna404()
    {
        var client = _factory.CreateClient();

        var resp = await client.PostAsJsonAsync("/api/clientes/pf", new CriarPessoaFisicaRequest
        {
            Nome = "Sem Filial",
            Email = "x@example.com",
            Telefone = "11111111111",
            FilialId = 99_999,
            Cpf = TestData.CpfValido1,
            DataNascimento = new DateTime(1992, 3, 3),
            RendaMensal = 4000m
        });

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CriarPf_CpfInvalido_Retorna400()
    {
        var client = _factory.CreateClient();
        var filialId = await CriarFilialAsync(client, "0012");

        var resp = await client.PostAsJsonAsync("/api/clientes/pf", new CriarPessoaFisicaRequest
        {
            Nome = "Inválido",
            Email = "x@example.com",
            Telefone = "11111111111",
            FilialId = filialId,
            Cpf = "111.111.111-11",
            DataNascimento = new DateTime(1990, 1, 1),
            RendaMensal = 1000m
        });

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
