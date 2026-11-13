using API_Solicitacoes.Data;
using API_Solicitacoes.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SolicitacoesDbContext>((services, options) =>
{
    var configuration = services.GetRequiredService<IConfiguration>();
    var environment = services.GetRequiredService<IWebHostEnvironment>();
    var connectionString = new SqliteConnectionStringBuilder(
        configuration.GetConnectionString("Solicitacoes")
        ?? throw new InvalidOperationException("A conexão com o SQLite não foi configurada."));
    if (!Path.IsPathRooted(connectionString.DataSource))
    {
        connectionString.DataSource = Path.Combine(environment.ContentRootPath, connectionString.DataSource);
    }

    options.UseSqlite(connectionString.ToString());
});
builder.Services.AddScoped<ISolicitacaoService, SolicitacaoService>();
builder.Services.AddHealthChecks();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Solicitações CAIXA", Version = "0.1.0" });
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    await scope.ServiceProvider.GetRequiredService<ISolicitacaoService>().InicializarAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");
app.MapControllers();
await app.RunAsync();

public partial class Program;
