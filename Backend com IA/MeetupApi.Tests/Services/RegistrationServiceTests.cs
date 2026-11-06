using FluentAssertions;
using MeetupApi.Web.Dtos;
using MeetupApi.Web.Exceptions;
using MeetupApi.Web.Models;
using MeetupApi.Web.Repositories;
using MeetupApi.Web.Services;
using Moq;

namespace MeetupApi.Tests.Services;

public class RegistrationServiceTests
{
    private readonly Mock<IRegistrationRepository> _registrationRepository;
    private readonly Mock<IEventRepository> _eventRepository;
    private readonly Mock<IParticipantRepository> _participantRepository;
    private readonly RegistrationService _registrationService;

    public RegistrationServiceTests()
    {
        _registrationRepository = new Mock<IRegistrationRepository>();
        _eventRepository = new Mock<IEventRepository>();
        _participantRepository = new Mock<IParticipantRepository>();
        _registrationService = CriarService(TimeProvider.System);

        // Por padrão o bloqueio do evento apenas executa a ação recebida
        _registrationRepository
            .Setup(r => r.ExecuteWithEventLock(It.IsAny<long>(), It.IsAny<Func<Registration>>()))
            .Returns((long _, Func<Registration> action) => action());
    }

    // -------------------------------------------------------------------------
    // INSCRIÇÃO — SUCESSO
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveCriarInscricaoComSucesso()
    {
        // Arrange
        DateTime dataFutura = DateTime.Now.AddDays(2);
        Event evento = CriarEvento(100, dataFutura);
        Participant participante = CriarParticipante();
        RegistrationRequest request = CriarInscricaoRequest();
        Registration inscricaoSalva = CriarInscricao(evento, participante, RegistrationStatus.ATIVA);

        _eventRepository.Setup(r => r.FindById(1L)).Returns(evento);
        _participantRepository.Setup(r => r.FindById(1L)).Returns(participante);
        _registrationRepository
            .Setup(r => r.ExistsByEventAndParticipantAndStatus(evento, participante, RegistrationStatus.ATIVA))
            .Returns(false);
        _registrationRepository
            .Setup(r => r.CountByEventAndStatus(evento, RegistrationStatus.ATIVA))
            .Returns(0L);
        _registrationRepository.Setup(r => r.Save(It.IsAny<Registration>())).Returns(inscricaoSalva);

        // Act
        RegistrationResponse response = _registrationService.Inscrever(1L, request);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be("ATIVA");
        response.EmailParticipante.Should().Be("ana@email.com");
        response.NomeParticipante.Should().Be("Ana Silva");
        _registrationRepository.Verify(r => r.Save(It.IsAny<Registration>()), Times.Once);
    }

    // -------------------------------------------------------------------------
    // INSCRIÇÃO — VALIDAÇÕES
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveLancarExcecaoQuandoEventoNaoExiste()
    {
        // Arrange
        _eventRepository.Setup(r => r.FindById(99L)).Returns((Event?)null);

        // Act
        Action act = () => _registrationService.Inscrever(99L, CriarInscricaoRequest());

        // Assert
        act.Should().Throw<ResourceNotFoundException>();
    }

    [Fact]
    public void DeveLancarExcecaoQuandoParticipanteNaoExiste()
    {
        // Arrange
        DateTime dataFutura = DateTime.Now.AddDays(2);
        Event evento = CriarEvento(100, dataFutura);

        _eventRepository.Setup(r => r.FindById(1L)).Returns(evento);
        _participantRepository.Setup(r => r.FindById(1L)).Returns((Participant?)null);

        // Act
        Action act = () => _registrationService.Inscrever(1L, CriarInscricaoRequest());

        // Assert
        act.Should().Throw<ResourceNotFoundException>();
    }

    [Fact]
    public void DeveLancarExcecaoQuandoEventoLotado()
    {
        // Arrange
        DateTime dataFutura = DateTime.Now.AddDays(2);
        Event evento = CriarEvento(50, dataFutura);
        Participant participante = CriarParticipante();
        RegistrationRequest request = CriarInscricaoRequest();

        _eventRepository.Setup(r => r.FindById(1L)).Returns(evento);
        _participantRepository.Setup(r => r.FindById(1L)).Returns(participante);
        _registrationRepository
            .Setup(r => r.ExistsByEventAndParticipantAndStatus(evento, participante, RegistrationStatus.ATIVA))
            .Returns(false);
        _registrationRepository
            .Setup(r => r.CountByEventAndStatus(evento, RegistrationStatus.ATIVA))
            .Returns(50L);

        // Act
        Action act = () => _registrationService.Inscrever(1L, request);

        // Assert
        act.Should().Throw<ConflictException>();
    }

    [Fact]
    public void DeveLancarExcecaoQuandoInscricaoDuplicada()
    {
        // Arrange
        DateTime dataFutura = DateTime.Now.AddDays(2);
        Event evento = CriarEvento(100, dataFutura);
        Participant participante = CriarParticipante();
        RegistrationRequest request = CriarInscricaoRequest();

        _eventRepository.Setup(r => r.FindById(1L)).Returns(evento);
        _participantRepository.Setup(r => r.FindById(1L)).Returns(participante);
        _registrationRepository
            .Setup(r => r.ExistsByEventAndParticipantAndStatus(evento, participante, RegistrationStatus.ATIVA))
            .Returns(true);

        // Act
        Action act = () => _registrationService.Inscrever(1L, request);

        // Assert
        act.Should().Throw<ConflictException>();
    }

    [Fact]
    public void DeveLancarExcecaoQuandoEventoNoPassado()
    {
        // Arrange
        DateTime dataPassada = DateTime.Now.AddDays(-1);
        Event evento = CriarEvento(100, dataPassada);

        _eventRepository.Setup(r => r.FindById(1L)).Returns(evento);

        // Act
        Action act = () => _registrationService.Inscrever(1L, CriarInscricaoRequest());

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DeveVerificarVagasESalvarDentroDoBloqueioDoEvento()
    {
        // Arrange
        Event evento = CriarEvento(100, DateTime.Now.AddDays(2));
        Participant participante = CriarParticipante();
        var chamadas = new List<string>();

        _eventRepository.Setup(r => r.FindById(1L)).Returns(evento);
        _participantRepository.Setup(r => r.FindById(1L)).Returns(participante);
        _registrationRepository
            .Setup(r => r.ExecuteWithEventLock(1L, It.IsAny<Func<Registration>>()))
            .Returns((long _, Func<Registration> action) =>
            {
                chamadas.Add("inicio-bloqueio");
                Registration resultado = action();
                chamadas.Add("fim-bloqueio");
                return resultado;
            });
        _registrationRepository
            .Setup(r => r.ExistsByEventAndParticipantAndStatus(evento, participante, RegistrationStatus.ATIVA))
            .Callback(() => chamadas.Add("duplicidade"))
            .Returns(false);
        _registrationRepository
            .Setup(r => r.CountByEventAndStatus(evento, RegistrationStatus.ATIVA))
            .Callback(() => chamadas.Add("capacidade"))
            .Returns(0L);
        _registrationRepository
            .Setup(r => r.Save(It.IsAny<Registration>()))
            .Callback(() => chamadas.Add("save"))
            .Returns(CriarInscricao(evento, participante, RegistrationStatus.ATIVA));

        // Act
        _registrationService.Inscrever(1L, CriarInscricaoRequest());

        // Assert
        chamadas.Should().Equal("inicio-bloqueio", "duplicidade", "capacidade", "save", "fim-bloqueio");
    }

    // -------------------------------------------------------------------------
    // LISTAGEM
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveListarInscricoesDoEvento()
    {
        // Arrange
        DateTime dataFutura = DateTime.Now.AddDays(2);
        Event evento = CriarEvento(100, dataFutura);
        Participant participante = CriarParticipante();
        Registration inscricao = CriarInscricao(evento, participante, RegistrationStatus.ATIVA);

        _eventRepository.Setup(r => r.ExistsById(1L)).Returns(true);
        _registrationRepository.Setup(r => r.FindByEventId(1L)).Returns(new List<Registration> { inscricao });

        // Act
        List<RegistrationResponse> responses = _registrationService.ListarInscricoes(1L);

        // Assert
        responses.Should().HaveCount(1);
        responses[0].EmailParticipante.Should().Be("ana@email.com");
        responses[0].Status.Should().Be("ATIVA");
    }

    [Fact]
    public void DeveLancarExcecaoQuandoListarInscricoesDeEventoInexistente()
    {
        // Arrange
        _eventRepository.Setup(r => r.ExistsById(99L)).Returns(false);

        // Act
        Action act = () => _registrationService.ListarInscricoes(99L);

        // Assert
        act.Should().Throw<ResourceNotFoundException>();
        _registrationRepository.Verify(r => r.FindByEventId(It.IsAny<long>()), Times.Never);
    }

    // -------------------------------------------------------------------------
    // CANCELAMENTO — SUCESSO
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveCancelarInscricaoComSucesso()
    {
        // Arrange
        DateTime dataFutura = DateTime.Now.AddDays(2);
        Event evento = CriarEvento(100, dataFutura);
        Participant participante = CriarParticipante();
        Registration inscricao = CriarInscricao(evento, participante, RegistrationStatus.ATIVA);
        Registration inscricaoCancelada = CriarInscricao(evento, participante, RegistrationStatus.CANCELADA);

        _registrationRepository.Setup(r => r.FindById(1L)).Returns(inscricao);
        _registrationRepository.Setup(r => r.Save(It.IsAny<Registration>())).Returns(inscricaoCancelada);

        // Act
        RegistrationResponse response = _registrationService.CancelarInscricao(1L, 1L);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be("CANCELADA");
        _registrationRepository.Verify(r => r.Save(inscricao), Times.Once);
    }

    // -------------------------------------------------------------------------
    // CANCELAMENTO — VALIDAÇÕES
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveLancarExcecaoQuandoCancelamentoTardio()
    {
        // Arrange
        DateTime dentroDePouco = DateTime.Now.AddHours(12);
        Event evento = CriarEvento(100, dentroDePouco);
        Participant participante = CriarParticipante();
        Registration inscricao = CriarInscricao(evento, participante, RegistrationStatus.ATIVA);

        _registrationRepository.Setup(r => r.FindById(1L)).Returns(inscricao);

        // Act
        Action act = () => _registrationService.CancelarInscricao(1L, 1L);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Cancelamento permitido somente até 24h antes do evento");
    }

    [Fact]
    public void DeveLancarExcecaoQuandoInscricaoNaoExiste()
    {
        // Arrange
        _registrationRepository.Setup(r => r.FindById(99L)).Returns((Registration?)null);

        // Act
        Action act = () => _registrationService.CancelarInscricao(1L, 99L);

        // Assert
        act.Should().Throw<ResourceNotFoundException>();
    }

    [Fact]
    public void DeveLancarExcecaoQuandoInscricaoJaCancelada()
    {
        // Arrange
        DateTime dataFutura = DateTime.Now.AddDays(2);
        Event evento = CriarEvento(100, dataFutura);
        Participant participante = CriarParticipante();
        Registration inscricaoCancelada = CriarInscricao(evento, participante, RegistrationStatus.CANCELADA);

        _registrationRepository.Setup(r => r.FindById(1L)).Returns(inscricaoCancelada);

        // Act
        Action act = () => _registrationService.CancelarInscricao(1L, 1L);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DeveLancarExcecaoQuandoInscricaoNaoPertenceAoEvento()
    {
        // Arrange
        DateTime dataFutura = DateTime.Now.AddDays(2);
        Event evento = CriarEvento(100, dataFutura);
        Participant participante = CriarParticipante();
        Registration inscricao = CriarInscricao(evento, participante, RegistrationStatus.ATIVA);

        _registrationRepository.Setup(r => r.FindById(1L)).Returns(inscricao);

        // Act
        Action act = () => _registrationService.CancelarInscricao(2L, 1L);

        // Assert
        act.Should().Throw<ResourceNotFoundException>();
        _registrationRepository.Verify(r => r.Save(It.IsAny<Registration>()), Times.Never);
    }

    [Fact]
    public void DevePermitirCancelamentoExatamente24HorasAntesDoEvento()
    {
        // Arrange
        var agora = new DateTime(2026, 11, 10, 10, 0, 0);
        RegistrationService service = CriarService(new FixedTimeProvider(agora));
        Event evento = CriarEvento(100, agora.AddHours(24));
        Participant participante = CriarParticipante();
        Registration inscricao = CriarInscricao(evento, participante, RegistrationStatus.ATIVA);

        _registrationRepository.Setup(r => r.FindById(1L)).Returns(inscricao);
        _registrationRepository.Setup(r => r.Save(inscricao)).Returns(inscricao);

        // Act
        RegistrationResponse response = service.CancelarInscricao(1L, 1L);

        // Assert
        response.Status.Should().Be("CANCELADA");
    }

    [Fact]
    public void DeveLancarExcecaoQuandoCancelamentoUmMinutoDepoisDoLimiteDe24Horas()
    {
        // Arrange
        var agora = new DateTime(2026, 11, 10, 10, 0, 0);
        RegistrationService service = CriarService(new FixedTimeProvider(agora));
        Event evento = CriarEvento(100, agora.AddHours(24).AddMinutes(-1));
        Participant participante = CriarParticipante();
        Registration inscricao = CriarInscricao(evento, participante, RegistrationStatus.ATIVA);

        _registrationRepository.Setup(r => r.FindById(1L)).Returns(inscricao);

        // Act
        Action act = () => service.CancelarInscricao(1L, 1L);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Cancelamento permitido somente até 24h antes do evento");
        _registrationRepository.Verify(r => r.Save(It.IsAny<Registration>()), Times.Never);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private RegistrationService CriarService(TimeProvider timeProvider) =>
        new(
            Microsoft.Extensions.Logging.Abstractions.NullLogger<RegistrationService>.Instance,
            _registrationRepository.Object,
            _eventRepository.Object,
            _participantRepository.Object,
            timeProvider);

    private static Event CriarEvento(int limiteVagas, DateTime dataHora)
    {
        var evento = new Event(".NET Summit 2026", "Conferência de .NET", dataHora, "São Paulo", limiteVagas);
        typeof(Event).GetProperty(nameof(Event.Id))!.SetValue(evento, 1L);
        return evento;
    }

    private static Participant CriarParticipante()
    {
        var participante = new Participant("Ana Silva", "ana@email.com");
        typeof(Participant).GetProperty(nameof(Participant.Id))!.SetValue(participante, 1L);
        return participante;
    }

    private static Registration CriarInscricao(Event evento, Participant participante, RegistrationStatus status)
    {
        var inscricao = new Registration(evento, participante, DateTime.Now, status);
        typeof(Registration).GetProperty(nameof(Registration.Id))!.SetValue(inscricao, 1L);
        return inscricao;
    }

    private static RegistrationRequest CriarInscricaoRequest() => new(1L);
}
