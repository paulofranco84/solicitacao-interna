using API_Solicitacoes.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Solicitacoes.Data;

public sealed class SolicitacoesDbContext(DbContextOptions<SolicitacoesDbContext> options) : DbContext(options)
{
    public DbSet<Solicitacao> Solicitacoes => Set<Solicitacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Solicitacao>(entity =>
        {
            entity.ToTable("solicitacoes");
            entity.HasKey(solicitacao => solicitacao.Id);
            entity.Property(solicitacao => solicitacao.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(solicitacao => solicitacao.Descricao).HasColumnName("descricao").IsRequired();
            entity.Property(solicitacao => solicitacao.Categoria).HasColumnName("categoria").IsRequired();
        });
    }
}
