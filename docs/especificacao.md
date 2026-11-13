# Solicitações CAIXA — especificação funcional

## Objetivo

Cadastrar solicitações internas contendo descrição e categoria, persistindo
os dados em SQLite e retornando o identificador gerado.

## Regras de entrada

- `POST /solicitacoes` recebe JSON com `descricao` e `categoria` do tipo string.
- Ambos os campos são obrigatórios; valores ausentes, `null`, vazios ou
  compostos somente por espaços em branco são rejeitados.
- Não existe catálogo fechado de categorias ou limite de tamanho definido
  pela aplicação.
- Os valores válidos são armazenados sem normalização ou remoção de espaços.
- Cada envio válido cria um novo registro, sem deduplicação.

## Critérios de aceite

| Cenário | Resultado esperado |
| --- | --- |
| JSON com descrição e categoria válidas | `201 Created`, corpo com `id` positivo e registro persistido |
| Descrição ausente, nula, vazia ou somente espaços | `400`, erro de descrição, sem gravação |
| Categoria ausente, nula, vazia ou somente espaços | `400`, erro de categoria, sem gravação |
| Ambos os campos inválidos | `400` com os dois erros, sem gravação |
| API em execução e `GET /health` | `200`, corpo `Healthy` em texto puro |

O health é liveness e não consulta dependências. A criação válida deve ser
testada separadamente para exercitar a gravação no banco.

## Escopo atual e limites

- Não há autenticação implementada nas rotas atuais.
- Não há consulta, listagem, edição ou exclusão de solicitações.
- O banco é criado na inicialização com `EnsureCreatedAsync`, sem migrations.
- Erros de infraestrutura não possuem resposta customizada definida.

## Referências

- [Contrato OpenAPI](openapi.yaml).
- [README: contrato, arquitetura, configuração, execução e evidências](../README.md).
- [Cenários automatizados](cenarios-teste.md).
