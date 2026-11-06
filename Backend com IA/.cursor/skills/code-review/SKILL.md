---
name: code-review
description: Performs a structured code review of C#/ASP.NET Core files following project standards. Returns a prioritised list of findings (CRÍTICA, ALTA, MÉDIA, BAIXA) with concrete fix suggestions. Use when the user asks for a code review, asks to review a file, or asks to check code quality.
disable-model-invocation: true
---

# Code Review — C# / ASP.NET Core

Analyse every C# file in scope against the checklist below. Return **only** findings — skip categories with no issues. If everything is fine, say so briefly.

## Output format

One block per finding:

```
[SEVERIDADE] Categoria — NomeDoArquivo.cs:linha
Problema: <o que está errado>
Sugestão: <correção concreta, com trecho de código quando útil>
```

Severities in descending priority: **CRÍTICA** | **ALTA** | **MÉDIA** | **BAIXA**

---

## Checklist

### 1 · Separação de responsabilidades
- Controllers delegam toda lógica de negócio ao Service — sem `if/else`, cálculos ou chamadas diretas ao Repository/DbContext.
- Services não montam respostas HTTP (`ActionResult`, `StatusCodes`).

```csharp
// ❌ lógica no controller
if (_repository.ExistsByTitulo(request.Titulo)) return Conflict();

// ✅ delegar ao service
return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
```

### 2 · Injeção de dependência
- Apenas injeção via construtor — dependências declaradas como `private readonly`.
- Tudo registrado no `Program.cs` (`AddScoped<IEventRepository, EventRepository>()`).

### 3 · Tratamento de exceções
- Exceções específicas (`ResourceNotFoundException`, `ConflictException`, `ArgumentException`) — nunca `Exception` nua.
- Mapeamento de exceção → HTTP centralizado no middleware de exceções — nenhum try/catch em Controller.

### 4 · Nomes descritivos
- Classes: substantivos que revelam propósito (`RegistrationService`, não `RegSvc`).
- Métodos: verbos com intenção clara (`ListarProximosEventos`, não `GetEvents2`).
- Variáveis: sem letras únicas nem abreviações (`participant`, não `p`).

### 5 · Persistência
- `SaveChanges` acontece nos métodos `Save`/`Delete` do Repository — Services não chamam o DbContext direto.
- Transações do EF (`BeginTransaction`) apenas quando há múltiplas escritas que precisam ser atômicas — não em leituras.
- Navegações carregadas com `Include` explícito — lazy loading desabilitado (evita N+1 escondido).

### 6 · Validações
- DataAnnotations (`[Required]`, `[EmailAddress]`, `[Range]`, etc.) nos parâmetros dos records de request.
- Regras de negócio (duplicidade, capacidade, datas) validadas no Service, não no Controller.
- Erros de ModelState retornam 422 com `ErrorResponse` (via `InvalidModelStateResponseFactory`).

### 7 · Segurança
- Nenhum dado sensível (senhas, tokens, connection strings) em DTOs de resposta ou logs.
- Todas as queries passam por EF Core/LINQ — sem SQL construído por concatenação de strings.
- Mensagem genérica em erros 500 — stacktrace nunca exposto ao cliente.

### 8 · Testes
- Cada método público cobre ao menos o caminho feliz **e** um caminho de falha.
- Nomes de teste em português no padrão `Deve<Acao><Contexto>` (ex.: `DeveLancarExcecaoQuandoEventoNaoExiste`).
- Testes unitários usam apenas Moq — sem `WebApplicationFactory`; testes de integração usam SQLite in-memory.
- Nenhuma lógica de produção duplicada nos testes — usar factory methods para dados de teste.
