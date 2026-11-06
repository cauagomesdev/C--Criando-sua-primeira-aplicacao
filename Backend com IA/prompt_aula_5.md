atualiza o devcontainer pra ter docker in docker pra rodar build, use a feature docker-outside-of-docker com "moby": false

Gere um .gitignore para um projeto C# com ASP.NET Core + dotnet CLI. 
Inclua: bin/, obj/, IDE files (.idea/, .vscode/ exceto extensions.json, *.user), arquivos de OS (.DS_Store, Thumbs.db), logs, *.log, e arquivo .env local.
NÃO ignore .devcontainer/ nem .cursor/ — queremos versionar essas pastas.

Sugira uma sequência de commits semânticos (Conventional Commits) para o estado atual do projeto. O projeto tem: DevContainer configurado, regras de IA (.ai/), entidades EF Core, DTOs, repositories, services implementados, controllers, middleware de exceções, testes unitários, skills e workflows. Extraia uma skill pra gerar commits relevantes, commit tem que ter apenas arquivos alterados na feature, crie uma branch a partir da main, nunca commit direto main, tem que seguir o padrao de mensagem sugerido.

Gere um README.md profissional para o projeto MeetUp API. Inclua:
1. Título e descrição curta do projeto
2. Stack utilizada (.NET 10, ASP.NET Core, Entity Framework Core, MySQL 8, SQLite in-memory nos testes, Docker, DevContainers)
3. Como rodar o projeto:
   a. Com DevContainers (recomendado) — abrir no VS Code/Cursor, Reopen in Container
   b. Com Docker Compose — docker compose up
   c. Localmente — pré-requisitos (.NET 10 SDK, MySQL), configurar appsettings.json / variáveis de ambiente
4. Endpoints disponíveis — tabela com: método HTTP, path, descrição, corpo esperado, código de sucesso
5. Exemplos de uso com curl (criar evento, listar, inscrever, cancelar)
6. Como rodar os testes (dotnet test)
7. Estrutura do projeto — árvore de diretórios com explicação de cada projeto/namespace
8. Sobre a pasta .ai/ — o que é, como usar as skills e workflows
9. Abordagem de desenvolvimento — breve descrição do fluxo test-first com IA

Preciso containerizar a MeetUp API (ASP.NET Core, .NET 10, dotnet CLI) com MySQL para ambiente de produção/staging.
Antes de gerar arquivos, planeje:
1. Dockerfile: estratégia multi-stage (build + runtime), imagem base para cada stage (mcr.microsoft.com/dotnet/sdk:10.0 no build, mcr.microsoft.com/dotnet/aspnet:10.0 no runtime), otimizações de cache de layers (restore antes do copy do código)
2. Docker Compose: serviços (api + mysql), variáveis de ambiente, volumes, networks, health checks, ordem de startup
3. .dockerignore: o que excluir do contexto de build
4. O que muda na configuração do ASP.NET Core quando roda em container vs. local (variáveis de ambiente para a connection string)
NÃO gere código ainda — só o plano.

Implemente o plano de containerização: gere o Dockerfile multi-stage, o .dockerignore e o docker-compose.yml de produção conforme planejado.
