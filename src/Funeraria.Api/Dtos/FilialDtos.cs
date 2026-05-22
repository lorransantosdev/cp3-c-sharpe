using System.ComponentModel.DataAnnotations;

namespace Funeraria.Api.Dtos;

public class CriarFilialRequest
{
    [Required, StringLength(10)]
    public string Numero { get; set; } = null!;

    [Required, StringLength(120)]
    public string Nome { get; set; } = null!;

    [Required, StringLength(200)]
    public string Endereco { get; set; } = null!;
}

public class FilialResponse
{
    public int Id { get; set; }
    public string Numero { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string Endereco { get; set; } = null!;
}
