using System.ComponentModel.DataAnnotations;

namespace Funeraria.Api.Dtos;

public class CriarServicoAssistencialRequest
{
    [Required, StringLength(120)]
    public string Nome { get; set; } = null!;

    [Required, StringLength(400)]
    public string Descricao { get; set; } = null!;

    [Range(0, 1)]
    public decimal TaxaPacoteBasico { get; set; }

    [Range(0, 1)]
    public decimal TaxaPacotePremium { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TaxaDeslocamento { get; set; }
}

public class CriarPlanoFunerarioRequest
{
    [Required, StringLength(120)]
    public string Nome { get; set; } = null!;

    [Required, StringLength(400)]
    public string Descricao { get; set; } = null!;

    [Range(0, 1)]
    public decimal MensalidadeBase { get; set; }

    [Range(1, 360)]
    public int CarenciaMaximaMeses { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CoberturaMaxima { get; set; }
}

public class ServicoResponse
{
    public int Id { get; set; }
    public string Tipo { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
}
