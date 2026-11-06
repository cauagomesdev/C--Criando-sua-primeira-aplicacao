---
name: add-feature-test-first
description: >-
  Adds a new feature to this ASP.NET Core project following a strict test-first
  (skeleton → unit tests → implementation) workflow. Use when the user says
  "adicionar feature", "nova feature", "add feature", "implementar endpoint",
  or provides a feature description with business rules and test scenarios.
disable-model-invocation: true
---

# Add Feature — Test-First Workflow

Guides the agent through adding a new feature to the MeetUp API in three
mandatory steps. **Never jump ahead**: complete each step and wait for
confirmation (or explicit "continue") before starting the next.

## Project conventions (always apply)

- DTOs are C# records; entities have explicit constructors
- Constructor injection only — everything registered in `Program.cs`
- DTO attribute names in **Portuguese**; entity properties in **English**
  mapped via `HasColumnName`/`ToTable` in `OnModelCreating`
- Exceptions: `ResourceNotFoundException` → 404, `ConflictException` → 409,
  `ArgumentException` → 422
- Unit tests: xUnit + Moq + FluentAssertions — mocks criados no construtor da
  classe de teste
- Integration tests: `WebApplicationFactory<Program>` com o DbContext trocado
  para SQLite in-memory; seed via Repository real; assertions no status HTTP
  e no JSON de resposta

---

## Prompt template

Copy the block below, replace every `{{VARIAVEL}}` with real content, then
send it as your next message. The agent will execute the steps in order.

````markdown
## Feature: {{DESCRICAO_FEATURE}}

**Endpoint:** `{{ENDPOINT}}`

**Regras de negócio:**
{{REGRAS_NEGOCIO}}

**Cenários de teste:**
{{CENARIOS_TESTE}}

---

### PASSO 1 — ESQUELETO

Gere apenas as assinaturas, **sem implementar lógica**:

1. **Repository** — declare o(s) método(s) necessário(s) na interface
   (`IEventRepository`, etc.) e implemente na classe correspondente usando o
   `MeetupDbContext` (LINQ — nunca SQL em string).
2. **Service** — adicione o método público com a assinatura correta; o corpo
   deve conter apenas `throw new NotImplementedException("not implemented");`.
3. **Controller** — adicione o endpoint (`[HttpGet]` / `[HttpPost]` /
   `[HttpPut]` / `[HttpDelete]` / `[HttpPatch]`) que delega a chamada ao
   Service e retorna o `ActionResult` com o status HTTP correto conforme as
   convenções do projeto.
4. **DTOs** — crie os records (`Request` / `Response`) somente se ainda não
   existirem ou se a feature exigir campos novos (DataAnnotations nos
   parâmetros do record).

Ao final do Passo 1 liste os arquivos criados/alterados e aguarde confirmação
para continuar.

---

### PASSO 2 — TESTES UNITÁRIOS

Com o esqueleto compilando, escreva os testes unitários do **Service**:

- Um `Mock<T>` por repositório/dependência usado pelo Service, criado no
  construtor da classe de teste e injetado no Service via construtor
- Cada cenário abaixo vira um método `[Fact]` nomeado em português no padrão
  `Deve<Acao><Contexto>`:

{{CENARIOS_TESTE}}

Regras para os testes:
- Use `mock.Setup(...).Returns(...)` para stubs e `mock.Verify(...)` para
  interações obrigatórias.
- Asserções com FluentAssertions (`Should()`, `Should().Throw<T>()`).
- Padrão AAA com comentários `// Arrange`, `// Act`, `// Assert`.
- **Não implemente a lógica do Service ainda.** Os testes devem falhar com
  `NotImplementedException` ao rodar agora.

Ao final do Passo 2 liste os cenários cobertos e aguarde confirmação para
continuar.

---

### PASSO 3 — TESTES DE INTEGRAÇÃO DO CONTROLLER

Com os testes unitários escritos, escreva os testes de integração do
**Controller** cobrindo os mesmos cenários:

Estrutura obrigatória da classe:
```csharp
public class {{NomeController}}Tests : IDisposable
{
    private static readonly JsonSerializerOptions Json = JsonSerializerOptions.Web;

    private readonly MeetupWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public {{NomeController}}Tests()
    {
        _factory = new MeetupWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
```

Regras para cada `[Fact]`:
- **Cenários de sucesso**: persista o estado necessário via Repository real
  (resolvido de um `IServiceScope` da factory) antes de realizar a requisição.
- **Cenários de erro de validação**: envie o body inválido e espere
  `HttpStatusCode.UnprocessableEntity` + `mensagem` presente no JSON.
- **Cenários de não encontrado**: use id fictício (ex.: `9999`) e espere
  `HttpStatusCode.NotFound` + `mensagem` presente.
- **Cenários de conflito**: persista o estado conflitante e espere
  `HttpStatusCode.Conflict` + `mensagem` presente.
- **Criação bem-sucedida**: além de `HttpStatusCode.Created`, verifique o
  header `Location` e os campos principais do body.
- **Deleção bem-sucedida**: espere apenas `HttpStatusCode.NoContent`.
- Nomeie os métodos em português no padrão `Deve<Acao><Contexto>`.
- Agrupe os testes por endpoint com um comentário separador:
  `// --- METHOD /caminho/do/endpoint ---`

Ao final do Passo 3 liste os cenários cobertos e aguarde confirmação para
continuar.

---

### PASSO 4 — IMPLEMENTAÇÃO

Implemente a lógica no Service para que **todos os testes dos Passos 2 e 3
passem**:

- Aplique cada regra de negócio listada:
{{REGRAS_NEGOCIO}}
- Lance `ResourceNotFoundException` para entidade não encontrada (404).
- Lance `ConflictException` para violações de unicidade/estado (409).
- Lance `ArgumentException` para violações de regra temporal/validação (422).
- Persista via Repository (`Save`/`Delete` chamam `SaveChanges`); use
  transação do EF (`BeginTransaction`) quando houver múltiplas escritas
  atômicas.
- Mapeie entidades ↔ DTOs dentro do Service — nunca no Controller.

Execute `dotnet test` ao final e confirme que **todos** os testes passam. Se
algum falhar, corrija a implementação antes de concluir.
````

---

## Checklist de revisão pós-geração

Após o Passo 4, valide cada item antes de considerar a feature pronta:

### Esqueleto
- [ ] Repository usa LINQ tipado — sem SQL em string
- [ ] Controller não contém lógica de negócio (só chama Service e monta
      `ActionResult`)
- [ ] DTOs são records com campos em português e DataAnnotations
      (`[Required]`, `[EmailAddress]`, `[Range]`, etc.)

### Testes unitários do Service (Passo 2)
- [ ] Todos os cenários de sucesso cobertos
- [ ] Todos os cenários de erro/exceção cobertos
- [ ] Asserções com FluentAssertions (`Should()`)
- [ ] `mock.Verify(r => r.Save(It.IsAny<T>()), Times.Never)` nos cenários que
      não devem persistir
- [ ] Nenhuma `WebApplicationFactory` — testes unitários usam apenas Moq

### Testes de integração do Controller (Passo 3)
- [ ] Classe usa `MeetupWebApplicationFactory` (SQLite in-memory)
- [ ] Dados de pré-condição inseridos via Repository real — nunca mocks
- [ ] Cenários de sucesso verificam status HTTP + campos relevantes do body
- [ ] Criação verifica o header `Location`
- [ ] Cenários de erro verificam status + campo `mensagem`
- [ ] Os mesmos cenários dos testes unitários estão presentes

### Implementação
- [ ] Service usa injeção por construtor (campo `readonly`)
- [ ] Persistência via Repository; transações do EF onde precisa
- [ ] Exceções corretas: `ResourceNotFoundException` (404) /
      `ConflictException` (409) / `ArgumentException` (422)
- [ ] Mapeamento entidade ↔ DTO feito exclusivamente no Service
- [ ] Todos os testes dos Passos 2 e 3 passam (`dotnet test`)
