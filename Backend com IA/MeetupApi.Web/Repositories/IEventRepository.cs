using MeetupApi.Web.Models;

namespace MeetupApi.Web.Repositories;

public interface IEventRepository
{
    Event? FindById(long id);
    bool ExistsById(long id);
    List<Event> FindAll();
    List<Event> FindByDateTimeAfterOrderByDateTimeAsc(DateTime dateTime);
    List<Event> FindByLocationContainingIgnoreCase(string location);
    Event Save(Event @event);
    void Delete(Event @event);
}
