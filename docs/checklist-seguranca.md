# Checklist CI e segurança · quinta


## Testes locais (CI)

```bash
dotnet restore .\API_Solicitacoes\
dotnet build .\API_Solicitacoes\  
dotnet test .\API_Solicitacoes.Tests\ --no-build
```

Pipeline: `.github/workflows/ci.yml`



| Item                                                           | ok? |
| -------------------------------------------------------------- | --- |
| Build e testes executados por outra pessoa usando o README     | sim |
| Sem senha/token no repositório                                | sim |
| .env no .gitignore · .env.example sem valores reais           | sim |
| Versões/dependências conhecidas · vulnerabilidades anotadas | sim |
| Logs e erros sem CPF ou dado sensível                         | sim |
| SAST / scan de dependências / secrets (se disponível)        | N/A |
| dotnet test verde local                                        | sim |
| GitHub Actions verde após push                                | sim |

Pendências e controles ainda manuais: nenhum
