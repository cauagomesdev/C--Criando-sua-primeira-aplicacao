using MeetupApi.Web.Models;

namespace MeetupApi.Web.Repositories;

public interface IParticipantRepository
{
    Participant? FindById(long id);
    List<Participant> FindAll();
    Participant? FindByEmail(string email);
    Participant Save(Participant participant);
    void Delete(Participant participant);
}
