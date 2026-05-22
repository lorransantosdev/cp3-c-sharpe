using System.Net;
using System.Net.Http.Json;
using Funeraria.Api.Dtos;
using Funeraria.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Funeraria.Tests;

public class ClientesPjTests : IClassFixture<FunerariaApiFactory>
{
    private readonly FunerariaApiFactory _factory;

    public ClientesPjTests(FunerariaApiFactory factory) => _factory = factory;

    private async Task<int> CriarFilialAsync(HttpClient client, string numero)
    {
        var resp = await client.PostAsJsonAsync("/api/filiais", new CriarFilialRequest
        {
            Numero = numero,
            Nome = "Filial PJ",
            Endereco = "Rua X, 100"
        });
        var f = await resp.Content.ReadFromJsonAsync<FilialResponse>();
        return f!.Id;
    }

    [Fact]
    public async Task CriarPj_Sucesso_RetornaCreated()
    {
        var client = _factory.CreateClient();
        var filialId = await CriarFilialAsync(client, "0020");

        var resp = await client.PostAsJsonAsync("/api/clientes/pj", new CriarPessoaJuridicaRequest
        {
            Nome = "Convênio XPTO",
            Email = "contato@xpto.com",
            Telefone = "1130000000",
            FilialId = filialId,
            Cnpj = TestData.CnpjValido1,
            RazaoSocial = "XPTO Convênios Ltda",
            FaturamentoMensal = 50000m
        });

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await resp.Content.ReadFromJsonAsync<ClienteResponse>();
        body!.Tipo.Should().Be("PJ");
        body.Cnpj.Should().Be("11222333000181");
    }

    [Fact]
    public async Task CriarPj_CnpjDuplicado_RetornaConflict()
    {
        var client = _factory.CreateClient();
        var filialId = await CriarFilialAsync(client, "0021");

        var req = new CriarPessoaJuridicaRequest
        {
            Nome = "Empresa A",
            Email = "a@example.com",
            Telefone = "1130000001",
            FilialId = filialId,
            Cnpj = TestData.CnpjValido2,
            RazaoSocial = "Empresa A Ltda",
            FaturamentoMensal = 25000m
        };

        var first = await client.PostAsJsonAsync("/api/clientes/pj", req);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await client.PostAsJsonAsync("/api/clientes/pj", req);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CriarPj_CnpjInvalido_Retorna400()
    {
        var client = _factory.CreateClient();
        var filialId = await CriarFilialAsync(client, "0022");

        var resp = await client.PostAsJsonAsync("/api/clientes/pj", new CriarPessoaJuridicaRequest
        {
            Nome = "Empresa Z",
            Email = "z@example.com",
            Telefone = "1130000002",
            FilialId = filialId,
            Cnpj = "00.000.000/0000-00",
            RazaoSocial = "Empresa Z",
            FaturamentoMensal = 10000m
        });

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
