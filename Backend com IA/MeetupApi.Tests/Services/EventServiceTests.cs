using FluentAssertions;
using MeetupApi.Web.Dtos;
using MeetupApi.Web.Exceptions;
using MeetupApi.Web.Models;
using MeetupApi.Web.Repositories;
using MeetupApi.Web.Services;
using Moq;

namespace MeetupApi.Tests.Services;

public class EventServiceTests
{
    private readonly Mock<IEventRepository> _eventRepository;
    private readonly EventService _eventService;

    public EventServiceTests()
    {
        _eventRepository = new Mock<IEventRepository>();
        _eventService = new EventService(
            Microsoft.Extensions.Logging.Abstractions.NullLogger<EventService>.Instance,
            _eventRepository.Object);
    }

    // -------------------------------------------------------------------------
    // CRIAÇÃO
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveCriarEventoComSucesso()
    {
        // Arrange
        EventRequest request = CriarEventoRequest();
        Event eventoSalvo = CriarEvento();
        _eventRepository.Setup(r => r.Save(It.IsAny<Event>())).Returns(eventoSalvo);

        // Act
        EventResponse response = _eventService.CriarEvento(request);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(1L);
        response.Titulo.Should().Be(".NET Summit 2026");
        response.Descricao.Should().Be("Conferência de .NET");
        response.Local.Should().Be("São Paulo");
        response.LimiteVagas.Should().Be(100);
        _eventRepository.Verify(r => r.Save(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public void DeveLancarExcecaoQuandoDataNoPassado()
    {
        // Arrange
        var request = new EventRequest(
            ".NET Summit 2026",
            "Conferência de .NET",
            DateTime.Now.AddDays(-1),
            "São Paulo",
            100);

        // Act
        Action act = () => _eventService.CriarEvento(request);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    // -------------------------------------------------------------------------
    // BUSCA
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveBuscarEventoPorIdComSucesso()
    {
        // Arrange
        Event evento = CriarEvento();
        _eventRepository.Setup(r => r.FindById(1L)).Returns(evento);

        // Act
        EventResponse response = _eventService.BuscarPorId(1L);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(1L);
        response.Titulo.Should().Be(".NET Summit 2026");
    }

    [Fact]
    public void DeveLancarExcecaoQuandoEventoNaoExiste()
    {
        // Arrange
        _eventRepository.Setup(r => r.FindById(99L)).Returns((Event?)null);

        // Act
        Action act = () => _eventService.BuscarPorId(99L);

        // Assert
        act.Should().Throw<ResourceNotFoundException>();
    }

    // -------------------------------------------------------------------------
    // BUSCA POR LOCAL
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveBuscarEventosPorLocal()
    {
        // Arrange
        Event evento = CriarEvento();
        _eventRepository
            .Setup(r => r.FindByLocationContainingIgnoreCase("paulo"))
            .Returns(new List<Event> { evento });

        // Act
        List<EventResponse> responses = _eventService.BuscarEventosPorLocal("paulo");

        // Assert
        responses.Should().HaveCount(1);
        responses[0].Local.Should().Be("São Paulo");
        _eventRepository.Verify(r => r.FindByLocationContainingIgnoreCase("paulo"), Times.Once);
    }

    [Fact]
    public void DeveRetornarListaVaziaQuandoLocalNaoEncontrado()
    {
        // Arrange
        _eventRepository
            .Setup(r => r.FindByLocationContainingIgnoreCase("Curitiba"))
            .Returns(new List<Event>());

        // Act
        List<EventResponse> responses = _eventService.BuscarEventosPorLocal("Curitiba");

        // Assert
        responses.Should().BeEmpty();
        _eventRepository.Verify(r => r.FindByLocationContainingIgnoreCase("Curitiba"), Times.Once);
    }

    // -------------------------------------------------------------------------
    // LISTAGEM
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveListarTodosEventos()
    {
        // Arrange
        var eventos = new List<Event> { CriarEvento() };
        _eventRepository.Setup(r => r.FindAll()).Returns(eventos);

        // Act
        List<EventResponse> responses = _eventService.ListarEventos();

        // Assert
        responses.Should().NotBeNull();
        responses.Should().HaveCount(1);
        responses[0].Titulo.Should().Be(".NET Summit 2026");
    }

    [Fact]
    public void DeveRetornarListaVaziaQuandoNaoHaEventos()
    {
        // Arrange
        _eventRepository.Setup(r => r.FindAll()).Returns(new List<Event>());

        // Act
        List<EventResponse> responses = _eventService.ListarEventos();

        // Assert
        responses.Should().NotBeNull();
        responses.Should().BeEmpty();
    }

    [Fact]
    public void DeveListarApenasEventosFuturos()
    {
        // Arrange
        var eventosFuturos = new List<Event> { CriarEvento() };
        DateTime? dataPassada = null;
        _eventRepository
            .Setup(r => r.FindByDateTimeAfterOrderByDateTimeAsc(It.IsAny<DateTime>()))
            .Callback<DateTime>(d => dataPassada = d)
            .Returns(eventosFuturos);

        DateTime antes = DateTime.Now;

        // Act
        List<EventResponse> responses = _eventService.ListarProximosEventos();

        // Assert
        _eventRepository.Verify(r => r.FindByDateTimeAfterOrderByDateTimeAsc(It.IsAny<DateTime>()), Times.Once);
        dataPassada.Should().NotBeNull();
        dataPassada.Should().BeOnOrAfter(antes);
        responses.Should().HaveCount(1);
    }

    // -------------------------------------------------------------------------
    // ATUALIZAÇÃO
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveAtualizarEventoComSucesso()
    {
        // Arrange
        Event eventoExistente = CriarEvento();
        EventRequest request = CriarEventoRequest();
        Event eventoAtualizado = CriarEvento();
        _eventRepository.Setup(r => r.FindById(1L)).Returns(eventoExistente);
        _eventRepository.Setup(r => r.Save(It.IsAny<Event>())).Returns(eventoAtualizado);

        // Act
        EventResponse response = _eventService.AtualizarEvento(1L, request);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(1L);
        _eventRepository.Verify(r => r.Save(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public void DeveLancarExcecaoAoAtualizarEventoInexistente()
    {
        // Arrange
        _eventRepository.Setup(r => r.FindById(99L)).Returns((Event?)null);

        // Act
        Action act = () => _eventService.AtualizarEvento(99L, CriarEventoRequest());

        // Assert
        act.Should().Throw<ResourceNotFoundException>();
    }

    // -------------------------------------------------------------------------
    // DELEÇÃO
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveDeletarEventoComSucesso()
    {
        // Arrange
        Event evento = CriarEvento();
        _eventRepository.Setup(r => r.FindById(1L)).Returns(evento);

        // Act
        _eventService.DeletarEvento(1L);

        // Assert
        _eventRepository.Verify(r => r.Delete(evento), Times.Once);
    }

    [Fact]
    public void DeveLancarExcecaoAoDeletarEventoInexistente()
    {
        // Arrange
        _eventRepository.Setup(r => r.FindById(99L)).Returns((Event?)null);

        // Act
        Action act = () => _eventService.DeletarEvento(99L);

        // Assert
        act.Should().Throw<ResourceNotFoundException>();
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static Event CriarEvento()
    {
        DateTime dataFutura = DateTime.Now.AddMonths(1);
        var evento = new Event(".NET Summit 2026", "Conferência de .NET", dataFutura, "São Paulo", 100);
        // simula ID gerado pelo banco via reflection
        typeof(Event).GetProperty(nameof(Event.Id))!.SetValue(evento, 1L);
        return evento;
    }

    private static EventRequest CriarEventoRequest() =>
        new(
            ".NET Summit 2026",
            "Conferência de .NET",
            DateTime.Now.AddMonths(1),
            "São Paulo",
            100);
}
