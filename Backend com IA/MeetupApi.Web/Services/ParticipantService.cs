using MeetupApi.Web.Dtos;
using MeetupApi.Web.Exceptions;
using MeetupApi.Web.Models;
using MeetupApi.Web.Repositories;

namespace MeetupApi.Web.Services;

public class ParticipantService
{
    private readonly IParticipantRepository _participantRepository;

    public ParticipantService(IParticipantRepository participantRepository)
    {
        _participantRepository = participantRepository;
    }

    public ParticipantResponse Cadastrar(ParticipantRequest request)
    {
        Participant? existing = _participantRepository.FindByEmail(request.Email);
        if (existing is not null)
        {
            throw new ConflictException($"Já existe um participante com o e-mail: {request.Email}");
        }

        var participant = new Participant(request.Nome, request.Email);
        Participant saved = _participantRepository.Save(participant);
        return ToResponse(saved);
    }

    public ParticipantResponse Atualizar(long id, ParticipantRequest request)
    {
        Participant participant = _participantRepository.FindById(id)
            ?? throw new ResourceNotFoundException($"Participante não encontrado com id: {id}");

        participant.Name = request.Nome;
        participant.Email = request.Email;

        Participant saved = _participantRepository.Save(participant);
        return ToResponse(saved);
    }

    public void Deletar(long id)
    {
        Participant participant = _participantRepository.FindById(id)
            ?? throw new ResourceNotFoundException($"Participante não encontrado com id: {id}");

        _participantRepository.Delete(participant);
    }

    private static ParticipantResponse ToResponse(Participant participant) =>
        new(participant.Id, participant.Name, participant.Email, participant.CreatedAt);
}
