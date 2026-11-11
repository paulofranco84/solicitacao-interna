using API_Solicitacoes.Data;
using API_Solicitacoes.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Solicitacoes.Services;

public sealed class SolicitacaoService(SolicitacoesDbContext dbContext) : ISolicitacaoService
{
    public async Task InicializarAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }

    public async Task<long> CriarAsync(string descricao, string categoria, CancellationToken cancellationToken = default)
    {
        var solicitacao = new Solicitacao
        {
            Descricao = descricao,
            Categoria = categoria
        };

        dbContext.Solicitacoes.Add(solicitacao);
        await dbContext.SaveChangesAsync(cancellationToken);
        return solicitacao.Id;
    }
}
