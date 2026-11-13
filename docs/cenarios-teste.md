# Cenários de teste

Os testes de integração usam `WebApplicationFactory` e SQLite em memória,
isolados do banco da execução local.

| Teste | Verificação |
| --- | --- |
| `Health_ApiEmExecucao_Retorna200Healthy` | HTTP 200, conteúdo `text/plain` e corpo `Healthy` |
| `Criar_DadosValidos_Retorna201EPersisteSolicitacao` | HTTP 201, identificador positivo e dados persistidos |
| `Criar_DescricaoVazia_Retorna400` | Descrição vazia ou somente espaços: HTTP 400 e nenhuma gravação |
| `Criar_SemCategoria_Retorna400` | Categoria ausente: HTTP 400 e nenhuma gravação |
| `Criar_CategoriaVazia_Retorna400` | Categoria vazia ou somente espaços: HTTP 400 e nenhuma gravação |

As duas teorias de campos vazios possuem dois casos cada, totalizando **7 casos
automatizados**. Código: [SolicitacoesTests.cs](../API_Solicitacoes.Tests/SolicitacoesTests.cs).
Os demais critérios da [especificação funcional](especificacao.md) não possuem
casos automatizados dedicados nesta versão.

Para execução, smoke HTTP com dados fictícios e evidências, consulte o
[README](../README.md) e as [requisições HTTP](../API_Solicitacoes/API_Solicitacoes.http).
