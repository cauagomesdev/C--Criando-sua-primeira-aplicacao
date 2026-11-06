using MeetupApi.Web.Models;

namespace MeetupApi.Web.Repositories;

public interface IRegistrationRepository
{
    Registration? FindById(long id);
    bool ExistsByEventAndParticipantAndStatus(Event @event, Participant participant, RegistrationStatus status);
    long CountByEventAndStatus(Event @event, RegistrationStatus status);
    List<Registration> FindByEventId(long eventId);
    Registration Save(Registration registration);
}
