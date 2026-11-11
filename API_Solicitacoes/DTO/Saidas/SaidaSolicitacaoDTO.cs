using System.Text.Json.Serialization;

namespace API_Solicitacoes.DTO.Saidas;

public sealed record SaidaSolicitacaoDTO([property: JsonPropertyName("id")] long Id);
