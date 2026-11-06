# MeetUp API — instruções para o Claude Code

Este projeto foi criado no curso usando o Cursor. As regras abaixo são as mesmas
da pasta `.cursor/rules` e são importadas daqui para o Claude seguir sempre.
Ao editar uma regra, altere o arquivo em `.cursor/rules`.

@.cursor/rules/project-stack.mdc
@.cursor/rules/csharp-code-style.mdc
@.cursor/rules/api-conventions.mdc

## Recursos do Claude Code neste projeto

- Skills em `.claude/skills` (cópia de `.cursor/skills`):
  - `/add-feature-test-first` — nova feature com testes primeiro
  - `/code-review` — revisão de código seguindo os padrões do projeto
  - `/semantic-commits` — commits no padrão Conventional Commits
- Comando `/add-feature` (`.claude/commands/add-feature.md`): roteiro completo
  para adicionar um endpoint, do planejamento ao README.
- O branch principal deste repositório se chama `master`.
