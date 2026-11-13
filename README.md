# Solicitação Interna

Desafio CAIXA para criação de uma API de solicitações internas.


## Integrantes do grupo

Thomas Gonzalez Miranda - c136762

Gabriel Marco de Souza e Silva - c118025

Janete Marilu Alves Boulhosa - c080459

Edinaldo do Nascimento Baiao - c141703

Pablo Vital Rodrigues de Andrade - c130883

Luiz Augusto de Oliveira Carvalho Silva - c079610

Ester Urbinati de Souza Dias - c134634

Paulo Henrique Zuliani Franco - c141101

Paulo Henrique dos Santos - c109563

Henrique Araujo Porto - c158493

## Visão geral

API HTTP em ASP.NET Core (.NET 8) para cadastrar solicitações internas,
com persistência em SQLite via Entity Framework Core.


| Método | Rota            | Finalidade                             | Resposta de sucesso       |
| ------- | --------------- | -------------------------------------- | ------------------------- |
| `GET`   | `/health`       | Verificar se a API está em execução | `200 Healthy`             |
| `POST`  | `/solicitacoes` | Criar uma solicitação interna        | `201` com o identificador |

URL local com o perfil `http`: `http://localhost:5245`.
As rotas atuais não exigem token nem cabeçalhos de autenticação. Não há endpoints
de consulta, listagem, atualização ou exclusão de solicitações.

Documentação do contrato:

- [OpenAPI 3.0.3](docs/openapi.yaml): especificação estática com schemas e exemplos.
- [Especificação funcional](docs/especificacao.md): regras e critérios de aceite.
- Swagger interativo em `Development`: [http://localhost:5245/swagger](http://localhost:5245/swagger).
- JSON gerado pelo Swagger em `Development`: [http://localhost:5245/swagger/v1/swagger.json](http://localhost:5245/swagger/v1/swagger.json).

O identificador `v1` do Swagger nomeia o documento; não existe prefixo `/v1`
nas rotas. O YAML é mantido separadamente, não é carregado pela aplicação.

## Contrato da API

### `POST /solicitacoes`

Cria uma solicitação. Envie um corpo JSON com `Content-Type: application/json`.


| Campo       | Tipo   | Obrigatório | Regra                                                                    |
| ----------- | ------ | ------------ | ------------------------------------------------------------------------ |
| `descricao` | string | Sim          | Não pode ser ausente,`null`, vazio ou conter somente espaços em branco |
| `categoria` | string | Sim          | Não pode ser ausente,`null`, vazio ou conter somente espaços em branco |

Não há lista fechada de categorias nem limite de tamanho definido pela aplicação.
Os valores aceitos são gravados como recebidos, sem remover espaços nas extremidades.

**Exemplo de requisição com dados fictícios:**

```http
POST /solicitacoes HTTP/1.1
Host: localhost:5245
Content-Type: application/json

{
  "descricao": "Smoke ficticio - solicitar material para o Hack",
  "categoria": "Teste"
}
```

**Sucesso — `201 Created`, `Content-Type: application/json`:**

```json
{
  "id": 1
}
```

`id` é um inteiro de 64 bits, positivo e gerado pelo banco. Seu valor varia
conforme os registros existentes. A resposta atual não contém cabeçalho
`Location`, e não existe rota de consulta pelo identificador.
Repetir o mesmo POST válido cria outro registro: não há deduplicação ou chave
de idempotência.

**Dados inválidos — `400 Bad Request`, `Content-Type: application/problem+json`:**

Exemplo de resposta para uma requisição sem `categoria`:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "categoria": ["A categoria é obrigatória."]
  },
  "traceId": "<identificador da requisicao>"
}
```

Quando `descricao` é inválida, a mensagem é `"A descrição é obrigatória."`
em `errors.descricao`. Se ambos os campos forem inválidos, a resposta contém
os dois erros. Nenhum registro é criado nesses casos.

JSON malformado, tipos incompatíveis ou corpo ausente também podem produzir
`400` pelo model binding do ASP.NET Core; as chaves e mensagens de `errors`
nesses casos são definidas pelo framework. Um tipo de conteúdo não suportado
pode resultar em `415 Unsupported Media Type`.

### `GET /health`

Não exige corpo nem autenticação.

**Sucesso — `200 OK`, `Content-Type: text/plain`:**

```text
Healthy
```

É um liveness sem verificações de dependências: um `200` não garante que o
SQLite continuará disponível para gravação. O smoke do POST complementa essa
verificação.

## Arquitetura e persistência

Veja o [diagrama de contexto](docs/diagrama-contexto.md) para os atores,
a fronteira da execução local e a separação entre o fluxo HTTP e o CI.
O [fluxo de eventos](docs/fluxo-eventos.md) detalha inicialização, validação,
persistência, respostas e health em diagramas de sequência.

Fluxo de criação:

```text
Cliente HTTP
  → SolicitacoesController (validação de entrada)
  → ISolicitacaoService / SolicitacaoService (criação)
  → SolicitacoesDbContext / EF Core
  → SQLite
  → resposta 201 com id
```

O banco e a tabela `solicitacoes` são criados com `EnsureCreatedAsync` na
inicialização. A tabela possui `id` (chave primária gerada), `descricao` e
`categoria` (obrigatórios). O arquivo permanece entre reinicializações.
Não há migrations ou estratégia de evolução de schema implementadas.

A conexão está em [appsettings.json](API_Solicitacoes/appsettings.json),
na chave `ConnectionStrings:Solicitacoes`, com padrão
`Data Source=solicitacoes.db`. Caminhos relativos são resolvidos a partir da
raiz do projeto da API. Para usar outro arquivo em uma execução local:

```powershell
$env:ConnectionStrings__Solicitacoes = 'Data Source=hack-local.db'
dotnet run --project .\API_Solicitacoes --no-build -c Release --launch-profile http
# Depois de parar a API com Ctrl+C:
Remove-Item Env:\ConnectionStrings__Solicitacoes
```

O exemplo usa `API_Solicitacoes\hack-local.db` e não remove esse arquivo.
Use somente dados fictícios. Falhas de inicialização do banco impedem a subida;
falhas de gravação não têm contrato de erro customizado nesta versão.

## Executar a API do Hack no PC do time

Pré-requisitos: SDK .NET 8, runtime ASP.NET Core 8 e acesso ao NuGet.
Execute os comandos na raiz deste repositório, em PowerShell.
O SQLite é criado automaticamente; não é necessário instalar um servidor de banco.

### Build e testes (mesmos passos do Actions)

```powershell
dotnet restore .\API_Solicitacoes
dotnet build .\API_Solicitacoes --no-restore -c Release
dotnet test .\API_Solicitacoes.Tests -c Release --verbosity normal
```

Só prossiga se cada comando terminar com código de saída zero. O teste compila
também o projeto de testes; não use `--no-build` antes dessa primeira compilação.

### Subir a API

```powershell
dotnet run --project .\API_Solicitacoes --no-build -c Release --launch-profile http
```

Aguarde `Now listening on: http://localhost:5245`. Mantenha esse terminal aberto.
O perfil `http` define a porta **5245** e o ambiente `Development`.
Swagger: [http://localhost:5245/swagger](http://localhost:5245/swagger).

O banco padrão fica em `API_Solicitacoes\solicitacoes.db`. Cada smoke válido grava
uma solicitação fictícia; não execute contra ambiente ou banco de produção.
Se a porta estiver ocupada, não encerre processos de outras pessoas: escolha uma
porta livre com `--urls http://localhost:5246` e ajuste a URL dos testes.

### Health e smoke do POST

Abra um segundo terminal PowerShell na raiz do repositório:

```powershell
$baseUrl = 'http://localhost:5245'

# Esperado: HTTP 200 e corpo Healthy
curl.exe --silent --show-error --max-time 10 --include "$baseUrl/health"

# Esperado: HTTP 201 e JSON {"id":<numero positivo>}
$valido = '{"descricao":"Smoke ficticio - solicitar material para o Hack","categoria":"Teste"}'
$valido | curl.exe --silent --show-error --max-time 10 --include --header 'Content-Type: application/json' --data-binary '@-' "$baseUrl/solicitacoes"

# Esperado: HTTP 400 e ValidationProblemDetails com errors.categoria
$invalido = '{"descricao":"Smoke ficticio - solicitar material para o Hack"}'
$invalido | curl.exe --silent --show-error --max-time 10 --include --header 'Content-Type: application/json' --data-binary '@-' "$baseUrl/solicitacoes"
```

Confira o status **e** o corpo: o `curl.exe` pode retornar código de saída zero
mesmo com HTTP 400/500. O 400 acima é proposital e não deve criar registro.
`GET /health` é um **liveness**: verifica que a API está em execução, não a
disponibilidade do SQLite. O POST válido exercita o fluxo HTTP → controller →
service → SQLite → resposta com `id`.

As mesmas requisições estão em [API_Solicitacoes.http](API_Solicitacoes/API_Solicitacoes.http).
Ao concluir, pare a API com `Ctrl+C` no primeiro terminal.

## GitHub Actions

Pipeline: [ci.yml](.github/workflows/ci.yml). Executa build e testes em .NET 8
nos pushes para `main`/`develop` e nos pull requests. Após publicar as alterações
por PR, confira se o workflow do **novo commit** terminou com `success`:
[execuções do Actions](https://github.com/paulofranco84/solicitacao-interna/actions).
Um verde anterior não valida alterações locais nem garante a execução de amanhã;
repita a conferência após cada mudança.

## Evidência de execução

Registre data/hora, commit, porta, status/corpos, fluxo e pendências após cada
execução. Não inclua dados pessoais, credenciais ou payloads reais.

### Validação local — 13/11/2026, 14:16 (UTC−03:00)

Base: commit `f4b2f46`, com as alterações locais deste incremento.
Ambiente: Windows, SDK 10.0.102, aplicação e testes em `net8.0`.
Foi usado um SQLite temporário isolado via `ConnectionStrings__Solicitacoes`,
sem alterar o banco padrão. A API foi parada e o banco temporário removido
ao terminar.


| Verificação           | Evidência                                                                                                                                                          |
| ----------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Build Release           | Sucesso, 0 avisos e 0 erros                                                                                                                                         |
| Testes                  | 7 aprovados, incluindo health, persistência do POST válido e ausência de gravação nos casos inválidos                                                         |
| Porta                   | `http://localhost:5245`                                                                                                                                             |
| Health                  | `GET /health` → `200`, `text/plain`, corpo `Healthy`                                                                                                               |
| Fluxo válido           | Dados fictícios do smoke acima →`POST /solicitacoes` → `201`, `application/json`, corpo `{"id":1}`                                                               |
| Fluxo inválido         | Categoria ausente →`400`, `application/problem+json`, `status: 400`, `errors.categoria: ["A categoria é obrigatória."]`                                          |
| Swagger                 | `GET /swagger/v1/swagger.json` → `200`, rota POST presente                                                                                                         |
| Actions consultado hoje | Última execução de`main`: [run 7](https://github.com/paulofranco84/solicitacao-interna/actions/runs/43287219855), `success`, em **12/11/2026**, commit `dfd66e6` |

**Pendências:**

- Publicar este incremento por PR e obter Actions verde no novo commit.
  Não há execução de hoje (13/11/2026) entre as execuções consultadas; o verde
  de ontem não cobre estas alterações. O Actions usa SDK 8, diferente do SDK
  local, portanto a validação no GitHub continua necessária.
- Cada integrante deve repetir o `dotnet run`, health e smoke no próprio PC e
  registrar a evidência; a execução acima valida somente este ambiente.
- Amanhã, conferir o workflow do commit mais recente e repetir o smoke antes
  da demonstração.
