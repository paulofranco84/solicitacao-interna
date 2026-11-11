using System.Text.Json.Serialization;

namespace API_Solicitacoes.DTO.Entradas;

public sealed class EntradaSolicitacaoDTO
{
    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }

    [JsonPropertyName("categoria")]
    public string? Categoria { get; set; }
}
