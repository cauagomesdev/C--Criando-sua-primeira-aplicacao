# MeetUp API

API REST para gerenciamento de eventos e inscrições de meetups. Organizadores criam eventos com limite de vagas; participantes se cadastram e se inscrevem, respeitando regras de negócio como capacidade máxima, inscrição única e cancelamento com antecedência mínima de 24h.

## Stack

| Tecnologia | Versão | Papel |
|---|---|---|
| .NET | 10 | Plataforma |
| ASP.NET Core Web API | 10 | Framework principal (controllers) |
| Entity Framework Core | 9 | Persistência (ORM) |
| Pomelo.EntityFrameworkCore.MySql | 9 | Provider MySQL do EF Core |
| MySQL | 8.0 | Banco de dados (produção) |
| SQLite in-memory | — | Banco de dados em memória (testes de integração) |
| xUnit + Moq + FluentAssertions | — | Testes |
| Docker + Docker Compose | — | Infraestrutura local |
| Dev Containers | — | Ambiente de desenvolvimento isolado |
| dotnet CLI | — | Build e gerenciamento de dependências |

## Como rodar o projeto

### Opção 1 — Dev Containers (recomendado)

Pré-requisitos: Docker + VS Code/Cursor com a extensão **Dev Containers**.

1. Abra a pasta do projeto no VS Code ou Cursor.
2. Aceite a sugestão **"Reopen in Container"** (ou rode o comando `Dev Containers: Reopen in Container`).
3. O container sobe com o SDK do .NET 10 e um MySQL 8 já configurado (`dotnet restore` roda automaticamente).
4. Rode a API:

```bash
dotnet run --project MeetupApi.Web
```

A API fica disponível em `http://localhost:8080`.

### Opção 2 — Docker Compose

Pré-requisito: Docker.

```bash
cp .env.example .env   # edite MYSQL_ROOT_PASSWORD
docker compose up --build
```

Sobe o MySQL 8 e a API (imagem multi-stage, usuário non-root) em `http://localhost:8080`.

### Opção 3 — Execução local

Pré-requisitos: SDK do .NET 10 e um MySQL 8 acessível.

1. Configure a connection string em `MeetupApi.Web/appsettings.json` ou via variável de ambiente:

```bash
export ConnectionStrings__DefaultConnection="Server=localhost;Port=3306;Database=meetup_db;User=root;Password=root"
```

2. Rode a API:

```bash
dotnet run --project MeetupApi.Web
```

## Endpoints

### Eventos — `/api/eventos`

| Método | Path | Descrição | Corpo | Status de sucesso |
|--------|------|-----------|-------|-------------------|
| `POST` | `/api/eventos` | Cria um novo evento | `EventRequest` | `201 Created` |
| `GET` | `/api/eventos` | Lista todos os eventos | — | `200 OK` |
| `GET` | `/api/eventos/{id}` | Busca evento por ID | — | `200 OK` |
| `GET` | `/api/eventos/busca?local=` | Busca eventos por local | — | `200 OK` |
| `GET` | `/api/eventos/proximos` | Lista eventos futuros | — | `200 OK` |
| `PUT` | `/api/eventos/{id}` | Atualiza um evento | `EventRequest` | `200 OK` |
| `DELETE` | `/api/eventos/{id}` | Remove um evento | — | `204 No Content` |

**`EventRequest`:**

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `titulo` | string | sim | Título do evento |
| `descricao` | string | não | Descrição do evento |
| `dataHora` | string (ISO 8601) | sim | Data e hora (deve ser futura) |
| `local` | string | não | Local onde ocorrerá o evento |
| `limiteVagas` | integer | sim | Número máximo de participantes (≥ 1) |

### Participantes — `/api/participantes`

| Método | Path | Descrição | Corpo | Status de sucesso |
|--------|------|-----------|-------|-------------------|
| `POST` | `/api/participantes` | Cadastra um participante | `ParticipantRequest` | `201 Created` |
| `PUT` | `/api/participantes/{id}` | Atualiza um participante | `ParticipantRequest` | `200 OK` |
| `DELETE` | `/api/participantes/{id}` | Remove um participante | — | `204 No Content` |

**`ParticipantRequest`:**

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `nome` | string | sim | Nome completo |
| `email` | string | sim | E-mail válido (único) |

### Inscrições — `/api/eventos/{eventoId}/inscricoes`

| Método | Path | Descrição | Corpo | Status de sucesso |
|--------|------|-----------|-------|-------------------|
| `POST` | `/api/eventos/{eventoId}/inscricoes` | Inscreve um participante no evento | `RegistrationRequest` | `201 Created` |
| `GET` | `/api/eventos/{eventoId}/inscricoes` | Lista inscrições do evento | — | `200 OK` |
| `PATCH` | `/api/eventos/{eventoId}/inscricoes/{inscricaoId}/cancelar` | Cancela uma inscrição | — | `200 OK` |

**`RegistrationRequest`:**

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `participanteId` | integer | sim | ID do participante a inscrever |

### Respostas de erro

| Status | Situação |
|--------|----------|
| `404 Not Found` | Recurso não encontrado |
| `409 Conflict` | Conflito de estado (ex.: participante já inscrito, evento lotado, e-mail duplicado) |
| `422 Unprocessable Entity` | Dados de entrada inválidos, data no passado ou cancelamento tardio |
| `500 Internal Server Error` | Erro inesperado (mensagem genérica, sem stacktrace) |

Corpo de erro: `{ "mensagem": "..." }`.

## Exemplos de uso com curl

### Criar um evento

```bash
curl -s -X POST http://localhost:8080/api/eventos \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "Tech Talk — ASP.NET Core 10",
    "descricao": "Novidades do .NET 10 e EF Core",
    "dataHora": "2026-12-10T19:00:00",
    "local": "São Paulo — Av. Paulista, 1000",
    "limiteVagas": 50
  }'
```

### Listar todos os eventos

```bash
curl -s http://localhost:8080/api/eventos
```

### Buscar próximos eventos

```bash
curl -s http://localhost:8080/api/eventos/proximos
```

### Buscar eventos por local

```bash
curl -s "http://localhost:8080/api/eventos/busca?local=São Paulo"
```

### Cadastrar um participante

```bash
curl -s -X POST http://localhost:8080/api/participantes \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Maria Silva",
    "email": "maria.silva@exemplo.com"
  }'
```

### Inscrever participante em um evento

```bash
curl -s -X POST http://localhost:8080/api/eventos/1/inscricoes \
  -H "Content-Type: application/json" \
  -d '{ "participanteId": 1 }'
```

### Listar inscrições de um evento

```bash
curl -s http://localhost:8080/api/eventos/1/inscricoes
```

### Cancelar uma inscrição

```bash
curl -s -X PATCH http://localhost:8080/api/eventos/1/inscricoes/1/cancelar
```

---

## Como rodar os testes

Os testes unitários usam Moq e os de integração usam `WebApplicationFactory` com SQLite em memória — nenhuma dependência externa é necessária.

```bash
dotnet test
```

Para rodar apenas os testes de uma camada específica:

```bash
# Somente testes unitários (Service)
dotnet test --filter "FullyQualifiedName~MeetupApi.Tests.Services"

# Somente testes de integração (Controller)
dotnet test --filter "FullyQualifiedName~MeetupApi.Tests.Controllers"
```

---

## Estrutura do projeto

```
meetup-api/
├── .cursor/                          # Configurações de IA para o Cursor/VS Code
│   ├── rules/                        # Regras aplicadas automaticamente ao agente
│   │   ├── api-conventions.mdc       # Convenções REST (status HTTP, validação, erros)
│   │   ├── csharp-code-style.mdc     # Estilo C# (records, construtores explícitos, DI por construtor)
│   │   └── project-stack.mdc         # Stack, arquitetura em camadas e convenções de nomes
│   ├── skills/                       # Skills (capacidades) do agente de IA
│   │   ├── add-feature-test-first/   # Skill para adicionar features com fluxo test-first
│   │   ├── code-review/              # Skill para revisão estruturada de código
│   │   └── semantic-commits/         # Skill para geração de Conventional Commits
│   └── workflows/                    # Workflows de alto nível para processos recorrentes
│       └── add-feature.md            # Workflow completo: planejar → esqueleto → testar → implementar
│
├── .devcontainer/                    # Configuração do Dev Container
│   ├── devcontainer.json             # Definição do container (.NET 10, extensões, portas)
│   └── docker-compose.yml            # Serviços: app (.NET) + mysql (MySQL 8)
│
├── MeetupApi.Web/                    # Projeto da API (ASP.NET Core)
│   ├── Controllers/                  # Controllers REST — apenas mapeamento HTTP
│   │   ├── EventController.cs
│   │   ├── ParticipantController.cs
│   │   └── RegistrationController.cs
│   ├── Services/                     # Regras de negócio
│   │   ├── EventService.cs
│   │   ├── ParticipantService.cs
│   │   └── RegistrationService.cs
│   ├── Repositories/                 # Repositórios (interface + classe sobre o DbContext)
│   │   ├── IEventRepository.cs / EventRepository.cs
│   │   ├── IParticipantRepository.cs / ParticipantRepository.cs
│   │   └── IRegistrationRepository.cs / RegistrationRepository.cs
│   ├── Models/                       # Entidades EF Core
│   │   ├── Event.cs
│   │   ├── Participant.cs
│   │   ├── Registration.cs
│   │   └── RegistrationStatus.cs
│   ├── Dtos/                         # Records de entrada/saída (campos em português)
│   │   ├── EventRequest.cs / EventResponse.cs
│   │   ├── ParticipantRequest.cs / ParticipantResponse.cs
│   │   ├── RegistrationRequest.cs / RegistrationResponse.cs
│   │   └── ErrorResponse.cs
│   ├── Exceptions/                   # Exceções de domínio
│   │   ├── ResourceNotFoundException.cs
│   │   └── ConflictException.cs
│   ├── Middlewares/
│   │   └── GlobalExceptionMiddleware.cs  # Mapeamento exceção → HTTP centralizado
│   ├── Data/
│   │   └── MeetupDbContext.cs        # DbContext (tabelas/colunas em português)
│   ├── Program.cs                    # Bootstrap, DI e configuração
│   └── appsettings.json              # Configuração principal (MySQL, logging)
│
├── MeetupApi.Tests/                  # Projeto de testes (xUnit)
│   ├── Services/                     # Testes unitários (Moq + FluentAssertions)
│   │   ├── EventServiceTests.cs
│   │   ├── ParticipantServiceTests.cs
│   │   └── RegistrationServiceTests.cs
│   └── Controllers/                  # Testes de integração (WebApplicationFactory + SQLite)
│       ├── MeetupWebApplicationFactory.cs
│       ├── EventControllerTests.cs
│       ├── ParticipantControllerTests.cs
│       └── RegistrationControllerTests.cs
│
├── Dockerfile                        # Imagem multi-stage (sdk:10.0 → aspnet:10.0, non-root)
├── docker-compose.yml                # Produção/staging: api + mysql
└── MeetupApi.slnx
```

---

## Sobre a pasta `.cursor/`

A pasta `.cursor/` contém instruções e ferramentas de IA que guiam o agente do Cursor durante o desenvolvimento. Ela é composta por três tipos de artefatos:

### Rules (`.cursor/rules/`)

Regras aplicadas automaticamente pelo agente em toda conversa. Definem as convenções do projeto: stack permitida, estilo de código C#, padrões REST e nomenclatura de campos. O agente as segue sem precisar ser lembrado a cada mensagem.

### Skills (`.cursor/skills/`)

Capacidades especializadas que o agente executa sob demanda. Para usar uma skill, mencione-a no chat:

| Skill | Como acionar | O que faz |
|-------|-------------|-----------|
| `add-feature-test-first` | _"Use a skill add-feature-test-first"_ | Guia a adição de features em 4 passos: esqueleto → testes unitários → testes de integração → implementação |
| `code-review` | _"Rode a skill de code review"_ | Revisa arquivos contra um checklist de severidades (CRÍTICA → BAIXA) |
| `semantic-commits` | _"Use a skill semantic-commits"_ | Gera Conventional Commits organizados por unidade lógica |

### Workflows (`.cursor/workflows/`)

Processos de alto nível que orquestram múltiplas skills e pontos de validação humana. O workflow `add-feature.md` descreve o ciclo completo de adição de um endpoint: planejar → esqueleto → testar → implementar → revisar → documentar.

---

## Abordagem de desenvolvimento

Este projeto adota um fluxo **test-first com assistência de IA**, operado diretamente no Cursor:

1. **Planejar** — O desenvolvedor descreve a feature em linguagem natural: endpoint, regras de negócio e cenários de teste.
2. **Esqueleto** — O agente gera assinaturas de método (repository, service, controller, DTOs) sem implementar lógica. O código compila, mas os métodos lançam `NotImplementedException`.
3. **Testes** — O agente escreve os testes unitários do Service (Moq + FluentAssertions) e os testes de integração do Controller (WebApplicationFactory + SQLite in-memory). Nesse ponto, todos os testes falham com `NotImplementedException`.
4. **Implementar** — O agente implementa a lógica no Service até que `dotnet test` fique verde.
5. **Revisar** — A skill `code-review` analisa os arquivos alterados contra o checklist do projeto e aponta findings por severidade.
6. **Documentar** — O README é atualizado com os novos endpoints.

Essa separação garante que os testes sejam escritos **antes** da lógica, eliminando o viés de escrever testes que se encaixam na implementação já existente.
