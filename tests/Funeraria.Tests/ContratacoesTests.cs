using System.Net;
using System.Net.Http.Json;
using Funeraria.Api.Dtos;
using Funeraria.Domain.Entities;
using Funeraria.Domain.Services;
using Funeraria.Infrastructure.Data;
using Funeraria.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Funeraria.Tests;

public class ContratacoesTests : IDisposable
{
    private readonly FunerariaApiFactory _factory;

    public ContratacoesTests() => _factory = new FunerariaApiFactory();

    public void Dispose() => _factory.Dispose();

    private async Task<(int filialId, int clienteId, int servicoId)> SetupAsync(HttpClient client, string sufixo)
    {
        var fl = await client.PostAsJsonAsync("/api/filiais", new CriarFilialRequest
        {
            Numero = $"03{sufixo}",
            Nome = "Filial Contrat",
            Endereco = "Rua Y"
        });
        var filial = (await fl.Content.ReadFromJsonAsync<FilialResponse>())!;

        var pf = await client.PostAsJsonAsync("/api/clientes/pf", new CriarPessoaFisicaRequest
        {
            Nome = "Cliente Contrat",
            Email = "c@example.com",
            Telefone = "11900000000",
            FilialId = filial.Id,
            Cpf = TestData.CpfValido1,
            DataNascimento = new DateTime(1990, 1, 1),
            RendaMensal = 10000m
        });
        var cliente = (await pf.Content.ReadFromJsonAsync<ClienteResponse>())!;

        var serv = await client.PostAsJsonAsync("/api/servicos/plano-funerario", new CriarPlanoFunerarioRequest
        {
            Nome = "Plano Familiar",
            Descricao = "Plano de assistência funerária",
            MensalidadeBase = 0.025m,
            CarenciaMaximaMeses = 24,
            CoberturaMaxima = 50000m
        });
        var servico = (await serv.Content.ReadFromJsonAsync<ServicoResponse>())!;

        return (filial.Id, cliente.Id, servico.Id);
    }

    [Fact]
    public async Task Contratacao_Valida_PublicaNaFila_RetornaAccepted()
    {
        var client = _factory.CreateClient();
        var (_, clienteId, servicoId) = await SetupAsync(client, "01");

        var resp = await client.PostAsJsonAsync("/api/contratacoes", new CriarContratacaoRequest
        {
            ClienteId = clienteId,
            ServicoId = servicoId,
            ValorSolicitado = 5000m,
            PrazoMeses = 12
        });

        resp.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var body = await resp.Content.ReadFromJsonAsync<ContratacaoResponse>();
        body!.Status.Should().Be(StatusContratacao.Pendente);
        _factory.Publisher.Mensagens.Should().Contain(m => m.ContratacaoId == body.Id);
    }

    [Fact]
    public async Task Contratacao_ClienteInexistente_Retorna404()
    {
        var client = _factory.CreateClient();
        var (_, _, servicoId) = await SetupAsync(client, "02");

        var resp = await client.PostAsJsonAsync("/api/contratacoes", new CriarContratacaoRequest
        {
            ClienteId = 99_999,
            ServicoId = servicoId,
            ValorSolicitado = 1000m,
            PrazoMeses = 6
        });

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Contratacao_ServicoInexistente_Retorna404()
    {
        var client = _factory.CreateClient();
        var (_, clienteId, _) = await SetupAsync(client, "03");

        var resp = await client.PostAsJsonAsync("/api/contratacoes", new CriarContratacaoRequest
        {
            ClienteId = clienteId,
            ServicoId = 99_999,
            ValorSolicitado = 1000m,
            PrazoMeses = 6
        });

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ConsultaStatus_AposProcessamento_RefleteResultado()
    {
        var client = _factory.CreateClient();
        var (_, clienteId, servicoId) = await SetupAsync(client, "04");

        var criar = await client.PostAsJsonAsync("/api/contratacoes", new CriarContratacaoRequest
        {
            ClienteId = clienteId,
            ServicoId = servicoId,
            ValorSolicitado = 5000m,
            PrazoMeses = 12
        });
        var contrat = (await criar.Content.ReadFromJsonAsync<ContratacaoResponse>())!;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FunerariaDbContext>();
            var entity = db.Contratacoes.Single(c => c.Id == contrat.Id);
            var cliente = db.Clientes.Single(c => c.Id == clienteId);
            var servico = db.Servicos.Single(p => p.Id == servicoId);

            var resultado = AnaliseContratacaoService.Analisar(cliente, servico, entity.ValorSolicitado, entity.PrazoMeses);
            entity.Status = resultado.Status;
            entity.Mensagem = resultado.Mensagem;
            entity.ScoreCalculado = resultado.Score;
            entity.TaxaAplicada = resultado.TaxaAplicada;
            entity.ProcessadaEm = DateTime.UtcNow;
            db.SaveChanges();
        }

        var get = await client.GetAsync($"/api/contratacoes/{contrat.Id}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = (await get.Content.ReadFromJsonAsync<ContratacaoResponse>())!;
        body.Status.Should().BeOneOf(StatusContratacao.Aprovada, StatusContratacao.Reprovada);
        body.ScoreCalculado.Should().NotBeNull();
        body.ProcessadaEm.Should().NotBeNull();
    }

    [Fact]
    public async Task ConsultaStatus_ContratacaoInexistente_Retorna404()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/api/contratacoes/99999");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
