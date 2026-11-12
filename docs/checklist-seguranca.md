# Checklist CI e segurança · quinta



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
