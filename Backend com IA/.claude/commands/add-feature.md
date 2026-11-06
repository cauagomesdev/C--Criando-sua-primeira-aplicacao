# Workflow: Adicionar Nova Feature

Processo completo para adicionar um endpoint novo à MeetUp API, do planejamento ao README atualizado. Siga as etapas **em ordem** — cada checkpoint de validação humana é obrigatório antes de avançar.

---

## Etapa 1 — PLANEJAR

### O que fazer

Descreva a feature em linguagem natural **antes de gerar qualquer código**. Preencha o template abaixo no chat:

```
## Feature: <nome da feature>

**Endpoint:** `METHOD /caminho`

**Regras de negócio:**
- <regra 1>
- <regra 2>
- ...

**Cenários de teste:**
- Sucesso: <descrição>
- Erro — não encontrado: <descrição>
- Erro — conflito/validação: <descrição>
- ...
```

### Skill

Nenhuma skill específica. Esta etapa é redação humana.

### ✅ Checkpoint — valide antes de seguir

- [ ] O endpoint está especificado com método HTTP e caminho completo (ex.: `GET /api/eventos/busca?local={local}`).
- [ ] Cada regra de negócio está descrita de forma precisa e testável.
- [ ] Todos os cenários de teste listados incluem ao menos um caminho feliz e um caminho de falha.
- [ ] Não há ambiguidade sobre qual entidade é criada/atualizada/deletada.

---

## Etapa 2 — ESQUELETO

### O que fazer

Com o planejamento aprovado, use a skill `add-feature-test-first` para gerar as assinaturas sem lógica:

1. Abra uma nova conversa no chat.
2. Copie o prompt template da skill (seção **Prompt template**).
3. Preencha `{{DESCRICAO_FEATURE}}`, `{{ENDPOINT}}`, `{{REGRAS_NEGOCIO}}` e `{{CENARIOS_TESTE}}` com o conteúdo do Planejar.
4. Envie apenas o bloco **PASSO 1 — ESQUELETO** do template.

O agente deve gerar:
- Método(s) na interface e na classe do **Repository** (LINQ sobre o `MeetupDbContext`)
- Método no **Service** com `throw new NotImplementedException("not implemented")`
- Endpoint no **Controller** delegando ao Service
- Records `Request` / `Response` (DTOs em português) se necessário

### Skill

`.claude/skills/add-feature-test-first/SKILL.md`

### ✅ Checkpoint — valide antes de seguir

- [ ] O projeto compila sem erros (`dotnet build`).
- [ ] O Controller não contém `if/else`, cálculos nem chamadas diretas ao Repository.
- [ ] Campos dos DTOs estão em **português** e anotados com DataAnnotations (`[Required]`, `[Range]`, etc.).
- [ ] Nenhuma lógica de negócio fora do Service.

---

## Etapa 3 — TESTAR

### O que fazer

Com o esqueleto compilando, gere os testes unitários do Service e os testes de integração do Controller enviando os blocos **PASSO 2** e **PASSO 3** do template da skill (em sequência, aguardando confirmação entre eles).

Os testes devem:
- Usar xUnit + Moq + FluentAssertions nos testes unitários
- Usar `MeetupWebApplicationFactory` (WebApplicationFactory + SQLite in-memory) nos testes de integração
- Falhar com `NotImplementedException` ao rodar agora (o Service ainda não foi implementado)
- Nomear métodos em português no padrão `Deve<Acao><Contexto>`

### Skill

`.claude/skills/add-feature-test-first/SKILL.md`

### ✅ Checkpoint — valide antes de seguir

- [ ] Todos os cenários do planejamento têm um `[Fact]` correspondente.
- [ ] Asserções usam FluentAssertions (`Should()`) — nenhum `Assert.True` / `Assert.Equal` cru.
- [ ] Testes unitários não sobem o host web (`WebApplicationFactory` ausente).
- [ ] Cenários de criação verificam o header `Location`.
- [ ] Cenários de erro verificam o campo `mensagem` do JSON.
- [ ] `dotnet test` falha **apenas** com `NotImplementedException` — nenhum erro de compilação.

---

## Etapa 4 — IMPLEMENTAR

### O que fazer

Envie o bloco **PASSO 4 — IMPLEMENTAÇÃO** do template da skill. O agente deve:

1. Substituir o `throw new NotImplementedException` pela lógica real no Service.
2. Aplicar todas as regras de negócio descritas no planejamento.
3. Usar `ResourceNotFoundException` para entidade não encontrada (→ 404).
4. Usar `ConflictException` para violações de unicidade/estado (→ 409).
5. Usar `ArgumentException` para violações de regra temporal/validação (→ 422).
6. Executar `dotnet test` e confirmar que **todos** os testes passam.

### Skill

`.claude/skills/add-feature-test-first/SKILL.md`

### ✅ Checkpoint — valide antes de seguir

- [ ] `dotnet test` verde — nenhum teste falhando.
- [ ] Mapeamento entidade ↔ DTO ocorre exclusivamente no Service.
- [ ] Controller permanece sem lógica de negócio após a implementação.
- [ ] Nenhuma query com concatenação de strings (risco de SQL injection).
- [ ] Nenhum dado sensível exposto no DTO de resposta.

---

## Etapa 5 — REVISAR

### O que fazer

Com os testes passando, peça ao agente uma revisão estruturada dos arquivos alterados:

```
@<NomeService>.cs @<NomeController>.cs @<NomeDTOs>.cs

Rode a skill de code review nesses arquivos.
```

A skill analisa os arquivos contra o checklist de severidades **CRÍTICA → ALTA → MÉDIA → BAIXA** e retorna apenas os findings — se não houver problemas, confirma que está tudo bem.

Corrija todos os findings **CRÍTICA** e **ALTA** antes de continuar. Findings **MÉDIA** e **BAIXA** podem ser endereçados como débito técnico no backlog.

### Skill

`.claude/skills/code-review/SKILL.md`

### ✅ Checkpoint — valide antes de seguir

- [ ] Nenhum finding CRÍTICA ou ALTA em aberto.
- [ ] `dotnet test` continua verde após as correções da revisão.
- [ ] Findings MÉDIA/BAIXA registrados no backlog se não forem corrigidos agora.

---

## Etapa 6 — DOCUMENTAR

### O que fazer

Atualize o `README.md` (ou crie-o se não existir) com os novos endpoints. Mantenha o padrão das seções existentes e inclua:

```markdown
### METHOD /caminho

Descrição em uma linha do que o endpoint faz.

**Request body** (se aplicável):
| Campo       | Tipo   | Obrigatório | Descrição              |
|-------------|--------|-------------|------------------------|
| `campo`     | string | sim         | Descrição do campo     |

**Respostas:**
| Status | Descrição                              |
|--------|----------------------------------------|
| 201    | Recurso criado — `Location` no header  |
| 404    | Entidade não encontrada                |
| 409    | Conflito de estado/unicidade           |
| 422    | Dados de entrada inválidos             |
```

### Skill

Nenhuma skill específica. Edição direta do `README.md`.

### ✅ Checkpoint — valide antes de seguir

- [ ] Todos os endpoints da feature estão documentados (incluindo paths de erro).
- [ ] Os campos do request body estão listados com tipo, obrigatoriedade e descrição.
- [ ] Os status HTTP de resposta batem com o que o Controller realmente retorna.
- [ ] O README compila sem broken links ou tabelas malformadas.

---

## Resumo rápido

| # | Etapa       | Quem age     | Skill                                  | Entrega                              |
|---|-------------|--------------|----------------------------------------|--------------------------------------|
| 1 | PLANEJAR    | Humano       | —                                      | Template de feature preenchido       |
| 2 | ESQUELETO   | Agente       | `add-feature-test-first`               | Código compila, sem lógica           |
| 3 | TESTAR      | Agente       | `add-feature-test-first`               | Testes escritos, falham na execução  |
| 4 | IMPLEMENTAR | Agente       | `add-feature-test-first`               | `dotnet test` verde                  |
| 5 | REVISAR     | Agente       | `code-review`                          | Sem findings CRÍTICA/ALTA            |
| 6 | DOCUMENTAR  | Humano/Agente| —                                      | README atualizado                    |
