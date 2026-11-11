using API_Solicitacoes.DTO.Entradas;
using API_Solicitacoes.DTO.Saidas;
using API_Solicitacoes.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_Solicitacoes.Controllers;

[ApiController]
[Route("solicitacoes")]
public sealed class SolicitacoesController(ISolicitacaoService solicitacaoService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<SaidaSolicitacaoDTO>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar(EntradaSolicitacaoDTO entrada, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entrada.Descricao))
        {
            ModelState.AddModelError("descricao", "A descrição é obrigatória.");
        }

        if (string.IsNullOrWhiteSpace(entrada.Categoria))
        {
            ModelState.AddModelError("categoria", "A categoria é obrigatória.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var id = await solicitacaoService.CriarAsync(entrada.Descricao!, entrada.Categoria!, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new SaidaSolicitacaoDTO(id));
    }
}
