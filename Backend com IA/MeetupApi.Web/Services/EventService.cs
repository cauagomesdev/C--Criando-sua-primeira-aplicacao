using MeetupApi.Web.Dtos;
using MeetupApi.Web.Exceptions;
using MeetupApi.Web.Models;
using MeetupApi.Web.Repositories;

namespace MeetupApi.Web.Services;

public class EventService
{
    private readonly ILogger<EventService> _logger;
    private readonly IEventRepository _eventRepository;

    public EventService(ILogger<EventService> logger, IEventRepository eventRepository)
    {
        _logger = logger;
        _eventRepository = eventRepository;
    }

    public EventResponse CriarEvento(EventRequest request)
    {
        _logger.LogDebug("Iniciando criação de evento: {Titulo}", request.Titulo);
        if (request.DataHora!.Value < DateTime.Now)
        {
            throw new ArgumentException("A data do evento não pode ser no passado");
        }

        var @event = new Event(
            request.Titulo,
            request.Descricao ?? string.Empty,
            request.DataHora.Value,
            request.Local ?? string.Empty,
            request.LimiteVagas!.Value);

        Event saved = _eventRepository.Save(@event);
        _logger.LogInformation("Evento criado com id={Id}", saved.Id);
        return ToResponse(saved);
    }

    public List<EventResponse> ListarEventos()
    {
        return _eventRepository.FindAll()
            .Select(ToResponse)
            .ToList();
    }

    public EventResponse BuscarPorId(long id)
    {
        Event @event = _eventRepository.FindById(id)
            ?? throw new ResourceNotFoundException($"Evento não encontrado com id: {id}");
        return ToResponse(@event);
    }

    public List<EventResponse> BuscarEventosPorLocal(string local)
    {
        return _eventRepository.FindByLocationContainingIgnoreCase(local)
            .Select(ToResponse)
            .ToList();
    }

    public List<EventResponse> ListarProximosEventos()
    {
        return _eventRepository.FindByDateTimeAfterOrderByDateTimeAsc(DateTime.Now)
            .Select(ToResponse)
            .ToList();
    }

    public EventResponse AtualizarEvento(long id, EventRequest request)
    {
        _logger.LogDebug("Iniciando atualização do evento id={Id}", id);
        Event @event = _eventRepository.FindById(id)
            ?? throw new ResourceNotFoundException($"Evento não encontrado com id: {id}");

        @event.Title = request.Titulo;
        @event.Description = request.Descricao ?? string.Empty;
        @event.DateTime = request.DataHora!.Value;
        @event.Location = request.Local ?? string.Empty;
        @event.Capacity = request.LimiteVagas!.Value;

        Event saved = _eventRepository.Save(@event);
        _logger.LogInformation("Evento atualizado com id={Id}", saved.Id);
        return ToResponse(saved);
    }

    public void DeletarEvento(long id)
    {
        _logger.LogDebug("Iniciando exclusão do evento id={Id}", id);
        Event @event = _eventRepository.FindById(id)
            ?? throw new ResourceNotFoundException($"Evento não encontrado com id: {id}");
        _eventRepository.Delete(@event);
        _logger.LogInformation("Evento deletado com id={Id}", id);
    }

    private static EventResponse ToResponse(Event @event)
    {
        int activeRegistrations = @event.Registrations
            .Count(r => r.Status == RegistrationStatus.ATIVA);

        return new EventResponse(
            @event.Id,
            @event.Title,
            @event.Description,
            @event.DateTime,
            @event.Location,
            @event.Capacity,
            @event.Capacity - activeRegistrations);
    }
}
