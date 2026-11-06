Me ajude a criar um arquivo de regras para um projeto C#. O arquivo deve ser um markdown que define convenções e restrições para um assistente de IA que vai me ajudar a programar. 

Contexto do projeto:
- API REST de gerenciamento de eventos/meetups
- .NET 10 + ASP.NET Core Web API + Entity Framework Core + MySQL (Pomelo.EntityFrameworkCore.MySql)
- Configuração com appsettings.json + variáveis de ambiente
- Arquitetura em camadas: Controller → Service → Repository (repositórios como classes com interface, usando o DbContext por baixo)
- DTOs como C# records
- Nomes de classes e métodos em inglês
- Nomes de atributos de domínio em português nos DTOs de entrada/saída (titulo, descricao, dataHora)
- Entidades EF Core com propriedades em inglês mapeadas para colunas em português via HasColumnName no OnModelCreating
- Validação com DataAnnotations ([Required], [MinLength], etc.) e checagem do ModelState
- Testes com xUnit + Moq (unitários) e WebApplicationFactory + SQLite in-memory (integração)
- Tratamento de erros centralizado com middleware de exceções
- Sem gerar boilerplate desnecessário — usar records para DTOs e construtores explícitos nas entidades
- Respostas da API sempre em JSON
- Códigos HTTP semânticos (201 para criação, 204 para delete, 409 para conflito, etc.)
- Injeção de dependência por construtor (registrada no Program.cs)

Crie a configuração de DevContainer para um projeto .NET 10 com ASP.NET Core.

Preciso de:
- Imagem base com o SDK do .NET 10
- Feature "ghcr.io/devcontainers/features/git:1" para git atualizado
- Feature "ghcr.io/devcontainers/features/github-cli:1" para o gh CLI
- MySQL 8 como serviço via Docker Compose
- Extensões do VS Code: C# Dev Kit e a extensão C# (ms-dotnettools.csdevkit, ms-dotnettools.csharp)
- Porta 8080 forwarded para a API
- Porta 3306 forwarded para o MySQL
- postCreateCommand que roda "dotnet restore" pra baixar dependências no build
- Variáveis de ambiente para conexão com o MySQL (MYSQL_ROOT_PASSWORD=root, MYSQL_DATABASE=meetup_db)

No docker-compose.yml do DevContainer:
- Serviço "app" com a imagem mcr.microsoft.com/devcontainers/dotnet:10.0 e command "sleep infinity"
- Serviço "mysql" com MySQL 8.0, variáveis de ambiente, volume nomeado para persistência, health check com mysqladmin ping

Gere o devcontainer.json e o docker-compose.yml separados. Lembre-se, precisamos do dotnet CLI também.
