using System.Text.Json.Serialization;

namespace Funeraria.Domain.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusContratacao
{
    Pendente = 0,
    EmAnalise = 1,
    Aprovada = 2,
    Reprovada = 3,
    Falhou = 4
}
