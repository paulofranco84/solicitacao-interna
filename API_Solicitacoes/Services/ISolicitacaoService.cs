namespace API_Solicitacoes.Services;

public interface ISolicitacaoService
{
    Task InicializarAsync(CancellationToken cancellationToken = default);
    Task<long> CriarAsync(string descricao, string categoria, CancellationToken cancellationToken = default);
}
