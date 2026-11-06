using MeetupApi.Web.Dtos;
using MeetupApi.Web.Exceptions;
using MeetupApi.Web.Models;
using MeetupApi.Web.Repositories;

namespace MeetupApi.Web.Services;

public class RegistrationService
{
    private readonly ILogger<RegistrationService> _logger;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IParticipantRepository _participantRepository;

    public RegistrationService(ILogger<RegistrationService> logger,
                               IRegistrationRepository registrationRepository,
                               IEventRepository eventRepository,
                               IParticipantRepository participantRepository)
    {
        _logger = logger;
        _registrationRepository = registrationRepository;
        _eventRepository = eventRepository;
        _participantRepository = participantRepository;
    }

    public RegistrationResponse Inscrever(long eventoId, RegistrationRequest request)
    {
        _logger.LogDebug("Iniciando inscrição no evento id={EventoId} para participante id={ParticipanteId}", eventoId, request.ParticipanteId);
        Event @event = _eventRepository.FindById(eventoId)
            ?? throw new ResourceNotFoundException($"Evento não encontrado: {eventoId}");

        if (@event.DateTime < DateTime.Now)
        {
            throw new ArgumentException("Não é possível se inscrever em um evento que já ocorreu");
        }

        Participant participant = _participantRepository.FindById(request.ParticipanteId!.Value)
            ?? throw new ResourceNotFoundException($"Participante não encontrado: {request.ParticipanteId}");

        if (_registrationRepository.ExistsByEventAndParticipantAndStatus(@event, participant, RegistrationStatus.ATIVA))
        {
            throw new ConflictException("Participante já inscrito neste evento");
        }

        long inscritosAtivos = _registrationRepository.CountByEventAndStatus(@event, RegistrationStatus.ATIVA);
        if (inscritosAtivos >= @event.Capacity)
        {
            throw new ConflictException("Evento sem vagas disponíveis");
        }

        var registration = new Registration(@event, participant, DateTime.Now, RegistrationStatus.ATIVA);
        Registration saved = _registrationRepository.Save(registration);
        _logger.LogInformation("Inscrição realizada com id={Id}", saved.Id);
        return ToResponse(saved);
    }

    public List<RegistrationResponse> ListarInscricoes(long eventoId)
    {
        return _registrationRepository.FindByEventId(eventoId)
            .Select(ToResponse)
            .ToList();
    }

    public RegistrationResponse CancelarInscricao(long eventoId, long inscricaoId)
    {
        _logger.LogDebug("Iniciando cancelamento da inscrição id={Id}", inscricaoId);
        Registration registration = _registrationRepository.FindById(inscricaoId)
            ?? throw new ResourceNotFoundException($"Inscrição não encontrada: {inscricaoId}");

        if (registration.Status == RegistrationStatus.CANCELADA)
        {
            throw new ArgumentException("Inscrição já está cancelada");
        }

        TimeSpan tempoAteEvento = registration.Event.DateTime - DateTime.Now;
        if (tempoAteEvento.TotalHours < 24)
        {
            throw new ArgumentException("Cancelamento permitido somente até 24h antes do evento");
        }

        registration.Status = RegistrationStatus.CANCELADA;
        Registration saved = _registrationRepository.Save(registration);
        _logger.LogInformation("Inscrição cancelada com id={Id}", saved.Id);
        return ToResponse(saved);
    }

    private static RegistrationResponse ToResponse(Registration registration) =>
        new(
            registration.Id,
            registration.Event.Id,
            registration.Participant.Name,
            registration.Participant.Email,
            registration.RegisteredAt,
            registration.Status.ToString());
}
