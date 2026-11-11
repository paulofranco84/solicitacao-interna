# API de solicitações

API de exercício em ASP.NET Core 8, Entity Framework Core e SQLite. A tabela é criada automaticamente na inicialização; os dados são persistidos no arquivo `solicitacoes.db` nesta pasta.

## Executar

Na raiz do projeto:

```powershell
dotnet run --project API_Solicitacoes --urls http://localhost:5245
```

O caminho do banco pode ser alterado pela configuração
`ConnectionStrings:Solicitacoes` ou pela variável de ambiente
`ConnectionStrings__Solicitacoes`. Caminhos relativos são resolvidos a partir
da pasta da API.

Em ambiente de desenvolvimento, a interface Swagger fica disponível em
[http://localhost:5245/swagger](http://localhost:5245/swagger), com o documento
gerado em [http://localhost:5245/swagger/v1/swagger.json](http://localhost:5245/swagger/v1/swagger.json).
O Swagger não é exposto em outros ambientes.

## Criar solicitação

A resposta é HTTP 201 com `{"id":1}`. Descrição e categoria são obrigatórias e não podem conter apenas espaços; entradas inválidas recebem HTTP 400. O contrato está em [openapi.yaml](../docs/openapi.yaml).

## Testes

```powershell
dotnet test API_Solicitacoes.Tests
```
