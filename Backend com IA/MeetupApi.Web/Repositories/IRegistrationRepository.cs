using MeetupApi.Web.Models;

namespace MeetupApi.Web.Repositories;

public interface IRegistrationRepository
{
    Registration? FindById(long id);
    bool ExistsByEventAndParticipantAndStatus(Event @event, Participant participant, RegistrationStatus status);
    long CountByEventAndStatus(Event @event, RegistrationStatus status);
    List<Registration> FindByEventId(long eventId);
    Registration Save(Registration registration);

    /// <summary>
    /// Executa a ação dentro de uma transação que mantém a linha do evento travada até o commit.
    /// Inscrições concorrentes no mesmo evento esperam umas pelas outras, então as checagens de
    /// duplicidade e capacidade feitas dentro da ação enxergam as inscrições já confirmadas.
    /// </summary>
    Registration ExecuteWithEventLock(long eventId, Func<Registration> action);
}
