Antes de escrever qualquer código, me ajude a planejar a estrutura da MeetUp API.

O sistema gerencia eventos e inscrições:
- Organizadores criam eventos com título, descrição, data/hora, local e limite de vagas
- Participantes se inscrevem em eventos (o participante seja uma entidade a parte)
- Regras: não inscrever em evento lotado, não permitir inscrição duplicada, não criar evento no passado, cancelamento só até 24h antes

Planeje:
1. Quais entidades EF Core e seus relacionamentos
2. Quais endpoints REST (método HTTP, path, request/response, códigos HTTP)
3. Quais DTOs (records) de entrada e saída
4. Estrutura de namespaces/pastas
5. Quais exceções customizadas
6. Quais cenários de teste para cada regra de negócio

Com base no plano que definimos, crie a solução e os projetos com o dotnet CLI:
- .NET 10 (LTS)
- Solução MeetupApi.slnx
- Projetos:
  - MeetupApi.Web (ASP.NET Core Web API, com controllers)
  - MeetupApi.Tests (xUnit)
- Pacotes NuGet no MeetupApi.Web:
  - Microsoft.EntityFrameworkCore
  - Pomelo.EntityFrameworkCore.MySql
- Pacotes NuGet no MeetupApi.Tests:
  - Moq
  - FluentAssertions
  - Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory)
  - Microsoft.EntityFrameworkCore.Sqlite (SQLite in-memory pros testes de integração)
- MeetupApi.Tests referencia MeetupApi.Web

Crie o Program.cs com minimal hosting (builder + app), registrando controllers e o DbContext, e configure o appsettings.json com a connection string do MySQL.

Seguindo o plano da MeetUp API, crie as entidades EF Core:
1. Evento - com Id (long, auto-gerado, identity), Titulo (string, obrigatório), Descricao (string, texto longo), DataHora (DateTime, obrigatório), Local (string), LimiteVagas (int, obrigatório), e uma coleção de inscrições (relação um-para-muitos)
2. Participante - com Id (long, auto-gerado, identity), Nome (string, obrigatório), Email (string, obrigatório, índice unique)
3. Inscricao - com Id (long, auto-gerado, identity), Evento (muitos-para-um, navegação com FK), Participante (muitos-para-um, navegação com FK), DataInscricao (DateTime), Status (enum StatusInscricao: ATIVA, CANCELADA)
4. O enum StatusInscricao separado

Lembre-se das regras do projeto: construtores explícitos (default + com argumentos), propriedades com getters, Equals/GetHashCode baseados apenas no Id. Cada entidade em seu próprio arquivo no namespace MeetupApi.Web.Models. Lembre-se que os nomes das FKs e das tabelas estão em inglês no mapeamento gerado, tem que ser em português como definido na regra — configure com HasColumnName/ToTable no OnModelCreating do DbContext.

Atualize o plano com o que ja foi feito, marque as tarefas done.

Crie a camada de repositório (classes com interface, usando o DbContext) para as entidades do projeto:
IEventoRepository / EventoRepository com:
- FindByDataHoraAfterOrderByDataHoraAsc(DateTime dataHora) → lista de eventos futuros
- métodos básicos de CRUD (FindById, FindAll, Save, Delete)
IParticipanteRepository / ParticipanteRepository com:
- FindByEmail(string email) → Participante? (nullable)
- métodos básicos de CRUD
IInscricaoRepository / InscricaoRepository com:
- ExistsByEventoAndParticipanteAndStatus(Evento evento, Participante participante, StatusInscricao status) → bool
- CountByEventoAndStatus(Evento evento, StatusInscricao status) → long
- FindByEventoId(long eventoId) → List<Inscricao>
Registre as interfaces no container de DI do Program.cs.

Crie os DTOs como C# records no namespace MeetupApi.Web.Dtos:
1. EventoRequest - record para criação/atualização:
   - string Titulo ([Required])
   - string Descricao
   - DateTime DataHora ([Required], data futura validada no Service)
   - string Local
   - int LimiteVagas ([Required] [Range(1, int.MaxValue)])
2. EventoResponse - record para retorno:
   - long Id, string Titulo, string Descricao, DateTime DataHora, string Local, int LimiteVagas, int VagasRestantes
3. InscricaoRequest - record para inscrição:
   - string Nome ([Required])
   - string Email ([Required] [EmailAddress])
4. InscricaoResponse - record para retorno:
   - long Id, string NomeParticipante, string EmailParticipante, DateTime DataInscricao, string Status
Lembre-se: os nomes dos campos no JSON de entrada/saída ficam em português (titulo, descricao, dataHora, local, limiteVagas, nome, email, vagasRestantes).

Na verdade temos que controlar também o registro do participante e referenciar no request da inscricao, a inscricao tem que referir o evento, registro nao contempla o evento.

Crie as Exceptions customizadas no namespace MeetupApi.Web.Exceptions:
1. ConflictException : Exception - para conflitos (evento lotado, inscrição duplicada)
2. ResourceNotFoundException : Exception - para recurso não encontrado
Ambas com construtor que recebe mensagem (string).

Crie os Services e Controllers como ESQUELETO — com assinaturas completas dos métodos, tipos de retorno corretos, atributos corretos, mas o corpo de cada método no Service deve conter apenas: `throw new NotImplementedException("Não implementado ainda");`.
O Controller pode delegar pro Service normalmente (já que o Service é quem tem a lógica pendente).

EventoService (registrado no DI) com:
- EventoResponse CriarEvento(EventoRequest request)
- List<EventoResponse> ListarEventos()
- EventoResponse BuscarPorId(long id)
- List<EventoResponse> ListarProximosEventos()
- EventoResponse AtualizarEvento(long id, EventoRequest request)
- void DeletarEvento(long id)

InscricaoService (registrado no DI) com:
- InscricaoResponse Inscrever(long eventoId, InscricaoRequest request)
- List<InscricaoResponse> ListarInscricoes(long eventoId)
- InscricaoResponse CancelarInscricao(long eventoId, long inscricaoId)

EventoController ([ApiController], [Route("api/eventos")]) com:
- POST / → 201 Created (usar CreatedAtAction com a URI no header Location)
- GET / → 200 OK
- GET /{id} → 200 OK
- GET /proximos → 200 OK
- PUT /{id} → 200 OK
- DELETE /{id} → 204 No Content

InscricaoController ([ApiController], [Route("api/eventos/{eventoId}/inscricoes")]) com:
- POST / → 201 Created
- GET / → 200 OK
- PATCH /{inscricaoId}/cancelar → 200 OK

Ambos os controllers com validação de ModelState nos [FromBody] (o [ApiController] já retorna 400/422 automático — configure pra devolver 422 em erro de validação) e injeção por construtor.
