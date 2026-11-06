Escreva testes unitários para o EventoService. O Service EXISTE mas todos os métodos lançam NotImplementedException — estou usando abordagem test-first.

Cenários de teste para o EventoService:

CRIAÇÃO:
1. DeveCriarEventoComSucesso - dados válidos → persiste e retorna EventoResponse com id gerado
2. DeveLancarExcecaoQuandoDataNoPassado - dataHora antes de agora → lança ArgumentException

BUSCA:
3. DeveBuscarEventoPorIdComSucesso - evento existe → retorna EventoResponse
4. DeveLancarExcecaoQuandoEventoNaoExiste - id inexistente → lança ResourceNotFoundException

LISTAGEM:
5. DeveListarTodosEventos - retorna lista de EventoResponse
6. DeveRetornarListaVaziaQuandoNaoHaEventos - sem eventos → retorna lista vazia
7. DeveListarApenasEventosFuturos - chama o repository com data atual

ATUALIZAÇÃO:
8. DeveAtualizarEventoComSucesso - evento existe → atualiza e retorna EventoResponse
9. DeveLancarExcecaoAoAtualizarEventoInexistente - id inexistente → lança ResourceNotFoundException

DELEÇÃO:
10. DeveDeletarEventoComSucesso - evento existe → deleta sem retorno
11. DeveLancarExcecaoAoDeletarEventoInexistente - id inexistente → lança ResourceNotFoundException

Use:
- xUnit + Moq
- Mock<IEventoRepository> criado no construtor da classe de teste, injetado no EventoService via construtor
- [Fact] em cada cenário
- Nomes dos testes em português descritivo (como listei acima, em PascalCase)
- FluentAssertions para assertions (Should())
- Padrão AAA (Arrange, Act, Assert) com comentários // Arrange // Act // Assert

Para os mocks, use mock.Setup(...).Returns(...) no Arrange. Para exceções, use Assert.Throws<T>(...) ou act.Should().Throw<T>().

Crie também os métodos auxiliares CriarEvento() e CriarEventoRequest() que geram objetos de teste.

Escreva testes unitários para o ParticipantService. O Service EXISTE mas todos os métodos lançam NotImplementedException — estou usando abordagem test-first.

Cenários para o método Cadastrar(ParticipantRequest request):

CADASTRO COM SUCESSO:
DeveCadastrarParticipanteComSucesso — email ainda não existe no banco (FindByEmail retorna null) → salva e retorna ParticipantResponse com os dados corretos

VALIDAÇÕES DE CADASTRO:
DeveLancarExcecaoQuandoEmailDuplicado — FindByEmail retorna um participante existente → lança ConflictException

Cenários para o método Atualizar(long id, ParticipantRequest request):

ATUALIZAÇÃO COM SUCESSO:
DeveAtualizarParticipanteComSucesso — FindById retorna o participante → atualiza os dados e retorna ParticipantResponse atualizado

VALIDAÇÕES DE ATUALIZAÇÃO:
DeveLancarExcecaoQuandoAtualizarParticipanteInexistente — FindById retorna null → lança ResourceNotFoundException

Cenários para o método Deletar(long id):

DELEÇÃO COM SUCESSO:
DeveDeletarParticipanteComSucesso — FindById retorna o participante → chama o repositório para deletar e não lança exceção

VALIDAÇÕES DE DELEÇÃO:
DeveLancarExcecaoQuandoDeletarParticipanteInexistente — FindById retorna null → lança ResourceNotFoundException

Use:
- xUnit + Moq
- Mock<IParticipantRepository> criado no construtor da classe de teste, injetado no ParticipantService
- Nomes em português descritivo (PascalCase)
- FluentAssertions para assertions
- Padrão AAA com comentários

Crie métodos auxiliares: CriarParticipante() (seta o Id via reflection ou setter interno, nome "Ana Silva", email "ana@email.com") e CriarParticipanteRequest() (retorna new ParticipantRequest("Ana Silva", "ana@email.com")).

Contexto das classes existentes:
- Participant — entidade EF Core com propriedades Name/Email e construtor Participant(string name, string email)
- ParticipantRequest — record com string Nome e string Email
- ParticipantResponse — record com long Id, string Nome, string Email, DateTime CriadoEm
- IParticipantRepository — interface do repositório com FindByEmail(string email) e CRUD básico
- ConflictException e ResourceNotFoundException — exceções de domínio já existentes no namespace MeetupApi.Web.Exceptions

Escreva testes unitários para o InscricaoService. O Service EXISTE mas todos os métodos lançam NotImplementedException — estou usando abordagem test-first.

Cenários para o método Inscrever(long eventoId, InscricaoRequest request):

INSCRIÇÃO COM SUCESSO:
1. DeveCriarInscricaoComSucesso - evento com vagas, participante já existe no banco → retorna InscricaoResponse com status ATIVA
2. DeveCriarParticipanteSeNaoExiste - email novo (participante não está no banco) → cria participante e depois inscreve

VALIDAÇÕES DE INSCRIÇÃO:
3. DeveLancarExcecaoQuandoEventoNaoExiste - eventoId inexistente → lança ResourceNotFoundException
4. DeveLancarExcecaoQuandoEventoLotado - contagem de inscrições ativas == limiteVagas → lança ConflictException
5. DeveLancarExcecaoQuandoInscricaoDuplicada - participante já tem inscrição ATIVA no evento → lança ConflictException
6. DeveLancarExcecaoQuandoEventoNoPassado - evento com dataHora antes de agora → lança ArgumentException

Cenários para o método ListarInscricoes(long eventoId):
7. DeveListarInscricoesDoEvento - retorna lista de InscricaoResponse

Cenários para o método CancelarInscricao(long eventoId, long inscricaoId):

CANCELAMENTO COM SUCESSO:
8. DeveCancelarInscricaoComSucesso - inscrição ATIVA e evento em mais de 24h → status muda pra CANCELADA

VALIDAÇÕES DE CANCELAMENTO:
9. DeveLancarExcecaoQuandoCancelamentoTardio - evento em menos de 24h → lança ArgumentException com mensagem "Cancelamento permitido somente até 24h antes do evento"
10. DeveLancarExcecaoQuandoInscricaoNaoExiste - inscricaoId inexistente → lança ResourceNotFoundException
11. DeveLancarExcecaoQuandoInscricaoJaCancelada - status já é CANCELADA → lança ArgumentException

Use:
- xUnit + Moq
- Mock<IEventoRepository>, Mock<IParticipanteRepository>, Mock<IInscricaoRepository> criados no construtor da classe de teste, injetados no InscricaoService
- Nomes em português descritivo (PascalCase)
- FluentAssertions para assertions
- Padrão AAA com comentários

Crie métodos auxiliares: CriarEvento(int limiteVagas, DateTime dataHora), CriarParticipante(), CriarInscricao(Evento, Participante, StatusInscricao), CriarInscricaoRequest().

Para testar a regra das 24h, use eventos com dataHora em DateTime.Now.AddDays(2) (sucesso) e DateTime.Now.AddHours(12) (falha).

Aqui estão os testes unitários do EventoService:
@MeetupApi.Tests/Services/EventServiceTests.cs 
E aqui está o esqueleto atual do EventoService (todos os métodos lançam NotImplementedException):
@MeetupApi.Web/Services/EventService.cs 

Implemente TODOS os métodos do EventoService para que todos os testes passem. 
Regras:
- Substitua cada throw NotImplementedException pela lógica real
- Use IEventoRepository (já injetado por construtor)
- Lance ResourceNotFoundException quando evento não for encontrado (FindById retorna null)
- Lance ArgumentException quando a data for no passado
- Para o EventoResponse, calcule vagasRestantes como: limiteVagas - número de inscrições ativas do evento
- Persista as alterações com SaveChanges nos métodos que alteram dados
é proibido alterar qualquer teste, eles tem que passar com a logica real implementada, as alteracoes de logica so devem ocorrer no servico

Aqui estão os testes unitários do ParticipantService:
@MeetupApi.Tests/Services/ParticipantServiceTests.cs 
E aqui está o esqueleto atual do ParticipantService (todos os métodos lançam NotImplementedException):
@MeetupApi.Web/Services/ParticipantService.cs 

Implemente TODOS os métodos do ParticipantService para que todos os testes passem.
Regras:
- Substitua cada throw new NotImplementedException pela lógica real
- Cadastrar(): verificar se já existe um participante com o mesmo email via FindByEmail. Se existir, lançar ConflictException. Caso contrário, criar um novo Participant e salvar.
- Atualizar(): buscar o participante por id via FindById. Se não encontrado, lançar ResourceNotFoundException. Caso encontrado, atualizar os dados (nome e email) e salvar.
- Deletar(): buscar o participante por id via FindById. Se não encontrado, lançar ResourceNotFoundException. Caso encontrado, chamar participantRepository.Delete(participante).
- Persista as alterações com SaveChanges nos métodos que alteram dados (Cadastrar, Atualizar, Deletar).

Aqui estão os testes unitários do InscricaoService:
@MeetupApi.Tests/Services/RegistrationServiceTests.cs 
E aqui está o esqueleto atual do InscricaoService (todos os métodos lançam NotImplementedException):
@MeetupApi.Web/Services/RegistrationService.cs 

Implemente TODOS os métodos do InscricaoService para que todos os testes passem.
Regras:
- Substitua cada throw NotImplementedException pela lógica real
- Inscrever(): verificar se evento existe, se não está no passado, se tem vagas (CountByEventoAndStatus < limiteVagas), se não é duplicada (ExistsByEventoAndParticipanteAndStatus). Se participante não existe (FindByEmail retorna null), criar um novo.
- CancelarInscricao(): verificar se inscrição existe, se não está já cancelada, se faltam mais de 24h para o evento (usar a diferença de DateTime como TimeSpan). Mudar status para CANCELADA.
- ListarInscricoes(): buscar por eventoId e converter para response
- Persista as alterações com SaveChanges; se uma operação envolver múltiplas escritas que precisam ser atômicas, use uma transação do EF (BeginTransaction)

baseado nos cenarios de testes do EventServiceTests, ParticipantServiceTests e RegistrationServiceTests precisamos testar todos os controladores para os mesmos servicos, a logica de servico ja foi implementada, crie um plan para os testes de integracao dos controllers, eles devem cobrir os mesmos cenarios dos testes unitarios, porem precisamos validar a captura das excecoes e mapeamento para os http codes corretos.
use WebApplicationFactory com o DbContext trocado pra SQLite in-memory nos testes, e configurar o logging pra exibir os SQLs que estao sendo executados (EnableSensitiveDataLogging + LogTo no DbContext de teste).
