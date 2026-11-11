namespace API_Solicitacoes.Models;

public sealed class Solicitacao
{
    public long Id { get; set; }
    public required string Descricao { get; set; }
    public required string Categoria { get; set; }
}
