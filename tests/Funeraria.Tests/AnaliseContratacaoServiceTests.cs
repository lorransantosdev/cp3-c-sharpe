using Funeraria.Domain.Entities;
using Funeraria.Domain.Services;
using FluentAssertions;
using Xunit;

namespace Funeraria.Tests;

public class AnaliseContratacaoServiceTests
{
    [Fact]
    public void Score_PfComBoaRenda_RetornaScoreElevado()
    {
        var pf = new PessoaFisica
        {
            Nome = "Teste",
            Email = "t@t.com",
            Telefone = "1",
            DataNascimento = new DateTime(1990, 1, 1),
            RendaMensal = 20_000m,
            CriadoEm = DateTime.UtcNow.AddYears(-2)
        };

        var score = AnaliseContratacaoService.CalcularScore(pf, 5_000m);
        score.Should().BeGreaterThan(700);
    }

    [Fact]
    public void PlanoFunerario_AcimaDaCobertura_Reprovado()
    {
        var pf = new PessoaFisica { Nome = "X", Email = "x@x", Telefone = "1", DataNascimento = new DateTime(1990,1,1), RendaMensal = 5000m };
        var plano = new PlanoFunerario { Nome = "P", Descricao = "d", CoberturaMaxima = 10_000m, CarenciaMaximaMeses = 24, MensalidadeBase = 0.02m };

        var r = AnaliseContratacaoService.Analisar(pf, plano, 50_000m, 12);
        r.Status.Should().Be(StatusContratacao.Reprovada);
    }

    [Fact]
    public void ServicoAssistencial_PjAltoFaturamento_AjustaTaxa()
    {
        var pj = new PessoaJuridica
        {
            Nome = "X",
            Email = "x@x",
            Telefone = "1",
            Cnpj = "11222333000181",
            RazaoSocial = "X",
            FaturamentoMensal = 250_000m,
            CriadoEm = DateTime.UtcNow.AddYears(-1)
        };
        var serv = new ServicoAssistencial
        {
            Nome = "S",
            Descricao = "d",
            TaxaPacotePremium = 0.0299m,
            TaxaPacoteBasico = 0.0149m,
            TaxaDeslocamento = 50m
        };

        var r = AnaliseContratacaoService.Analisar(pj, serv, 0m, null);
        r.Status.Should().Be(StatusContratacao.Aprovada);
        r.TaxaAplicada.Should().BeLessThan(0.0299m);
    }
}
