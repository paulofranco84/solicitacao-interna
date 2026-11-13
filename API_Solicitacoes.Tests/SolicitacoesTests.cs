using System.Net;
using System.Net.Http.Json;
using API_Solicitacoes.DTO.Saidas;
using Microsoft.Data.Sqlite;

namespace API_Solicitacoes.Tests;

public sealed class SolicitacoesTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly SolicitacoesWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SolicitacoesTests()
    {
        _connection.Open();
        _factory = new SolicitacoesWebApplicationFactory(_connection);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Health_ApiEmExecucao_Retorna200Healthy()
    {
        using var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Criar_DadosValidos_Retorna201EPersisteSolicitacao()
    {
        var response = await _client.PostAsJsonAsync("/solicitacoes", new
        {
            descricao = "Solicitar atendimento",
            categoria = "Atendimento"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var saida = await response.Content.ReadFromJsonAsync<SaidaSolicitacaoDTO>();
        Assert.NotNull(saida);
        Assert.True(saida.Id > 0);

        await using var command = _connection.CreateCommand();
        command.CommandText = "SELECT descricao, categoria FROM solicitacoes WHERE id = $id";
        command.Parameters.AddWithValue("$id", saida.Id);
        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("Solicitar atendimento", reader.GetString(0));
        Assert.Equal("Atendimento", reader.GetString(1));
        Assert.False(await reader.ReadAsync());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Criar_DescricaoVazia_Retorna400(string descricao)
    {
        var response = await _client.PostAsJsonAsync("/solicitacoes", new { descricao, categoria = "Atendimento" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertNenhumaSolicitacaoAsync();
    }

    [Fact]
    public async Task Criar_SemCategoria_Retorna400()
    {
        var response = await _client.PostAsJsonAsync("/solicitacoes", new { descricao = "Solicitar atendimento" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertNenhumaSolicitacaoAsync();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Criar_CategoriaVazia_Retorna400(string categoria)
    {
        var response = await _client.PostAsJsonAsync("/solicitacoes", new { descricao = "Solicitar atendimento", categoria });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertNenhumaSolicitacaoAsync();
    }

    private async Task AssertNenhumaSolicitacaoAsync()
    {
        await using var command = _connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM solicitacoes";
        Assert.Equal(0L, (long)(await command.ExecuteScalarAsync())!);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
        _connection.Dispose();
    }
}
