using API_Solicitacoes.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace API_Solicitacoes.Tests;

public sealed class SolicitacoesWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public SolicitacoesWebApplicationFactory(SqliteConnection connection)
    {
        _connection = connection;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<SolicitacoesDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.AddDbContext<SolicitacoesDbContext>(options => options.UseSqlite(_connection));
        });
    }
}
