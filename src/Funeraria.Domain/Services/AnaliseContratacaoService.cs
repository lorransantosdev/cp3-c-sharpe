using Funeraria.Domain.Entities;

namespace Funeraria.Domain.Services;

public record ResultadoAnalise(
    StatusContratacao Status,
    string Mensagem,
    int Score,
    decimal? TaxaAplicada);

public static class AnaliseContratacaoService
{
    public static int CalcularScore(Cliente cliente, decimal valorSolicitado)
    {
        var score = 500;

        var idadeRelacionamentoDias = (int)(DateTime.UtcNow - cliente.CriadoEm).TotalDays;
        score += Math.Min(idadeRelacionamentoDias / 30, 100);

        switch (cliente)
        {
            case PessoaFisica pf:
                if (pf.RendaMensal > 0)
                {
                    var razao = pf.RendaMensal == 0 ? 0 : valorSolicitado / pf.RendaMensal;
                    score += razao switch
                    {
                        < 1 => 200,
                        < 3 => 120,
                        < 6 => 50,
                        < 12 => 0,
                        _ => -150
                    };
                }
                var idade = (int)((DateTime.UtcNow - pf.DataNascimento).TotalDays / 365.25);
                if (idade is >= 25 and <= 60) score += 50;
                break;

            case PessoaJuridica pj:
                if (pj.FaturamentoMensal >= 100_000) score += 200;
                else if (pj.FaturamentoMensal >= 30_000) score += 120;
                else if (pj.FaturamentoMensal >= 10_000) score += 60;
                break;
        }

        return Math.Clamp(score, 0, 1000);
    }

    public static ResultadoAnalise Analisar(Cliente cliente, Servico servico, decimal valorSolicitado, int? prazoMeses)
    {
        var score = CalcularScore(cliente, valorSolicitado);

        return servico switch
        {
            PlanoFunerario p => AnalisarPlanoFunerario(cliente, p, valorSolicitado, prazoMeses, score),
            ServicoAssistencial s => AnalisarServicoAssistencial(cliente, s, score),
            Jazigo j => AnalisarJazigo(cliente, j, score),
            _ => new ResultadoAnalise(StatusContratacao.Falhou, "Serviço não suportado", score, null)
        };
    }

    private static ResultadoAnalise AnalisarPlanoFunerario(Cliente cliente, PlanoFunerario p, decimal valor, int? prazo, int score)
    {
        if (valor <= 0)
            return new(StatusContratacao.Reprovada, "Cobertura solicitada inválida", score, null);
        if (valor > p.CoberturaMaxima)
            return new(StatusContratacao.Reprovada, $"Cobertura acima do máximo do plano (R$ {p.CoberturaMaxima:N2})", score, null);
        if (prazo is null || prazo <= 0 || prazo > p.CarenciaMaximaMeses)
            return new(StatusContratacao.Reprovada, $"Carência inválida — máximo {p.CarenciaMaximaMeses} meses", score, null);

        if (score < 400)
            return new(StatusContratacao.Reprovada, $"Score insuficiente ({score})", score, null);

        var mensalidadeBase = p.MensalidadeBase;
        var ajuste = score switch
        {
            >= 800 => -0.0050m,
            >= 700 => -0.0030m,
            >= 600 => -0.0010m,
            >= 500 => 0m,
            _ => 0.0050m
        };
        var mensalidadeAplicada = Math.Max(0.0010m, mensalidadeBase + ajuste);

        return new(StatusContratacao.Aprovada, $"Plano funerário aprovado — mensalidade {mensalidadeAplicada:P2} sobre a cobertura", score, mensalidadeAplicada);
    }

    private static ResultadoAnalise AnalisarServicoAssistencial(Cliente cliente, ServicoAssistencial s, int score)
    {
        if (score < 350)
            return new(StatusContratacao.Reprovada, $"Score insuficiente para contratação do serviço ({score})", score, null);

        var taxaPremium = s.TaxaPacotePremium;
        var faturamento = cliente switch
        {
            PessoaJuridica pj => pj.FaturamentoMensal,
            PessoaFisica pf => pf.RendaMensal,
            _ => 0m
        };

        var desconto = faturamento switch
        {
            >= 200_000 => -0.0100m,
            >= 50_000 => -0.0050m,
            >= 10_000 => -0.0020m,
            _ => 0m
        };
        var taxaFinal = Math.Max(0.0050m, taxaPremium + desconto);

        return new(StatusContratacao.Aprovada, $"Serviço assistencial aprovado — taxa pacote premium {taxaFinal:P2}", score, taxaFinal);
    }

    private static ResultadoAnalise AnalisarJazigo(Cliente cliente, Jazigo j, int score)
    {
        if (string.IsNullOrWhiteSpace(j.CnpjCemiterio))
            return new(StatusContratacao.Falhou, "CNPJ do cemitério não informado", score, null);
        return new(StatusContratacao.Aprovada, "Jazigo reservado", score, null);
    }
}
