using MeetupApi.Web.Data;
using MeetupApi.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetupApi.Web.Repositories;

public class EventRepository : IEventRepository
{
    private readonly MeetupDbContext _context;

    public EventRepository(MeetupDbContext context)
    {
        _context = context;
    }

    public Event? FindById(long id) =>
        _context.Events
            .Include(@event => @event.Registrations)
            .FirstOrDefault(@event => @event.Id == id);

    public bool ExistsById(long id) =>
        _context.Events.Any(@event => @event.Id == id);

    public List<Event> FindAll() =>
        _context.Events
            .Include(@event => @event.Registrations)
            .ToList();

    public List<Event> FindByDateTimeAfterOrderByDateTimeAsc(DateTime dateTime) =>
        _context.Events
            .Include(@event => @event.Registrations)
            .Where(@event => @event.DateTime > dateTime)
            .OrderBy(@event => @event.DateTime)
            .ToList();

    public List<Event> FindByLocationContainingIgnoreCase(string location) =>
        _context.Events
            .Include(@event => @event.Registrations)
            .Where(@event => @event.Location.ToLower().Contains(location.ToLower()))
            .ToList();

    public Event Save(Event @event)
    {
        if (@event.Id == 0)
        {
            _context.Events.Add(@event);
        }
        _context.SaveChanges();
        return @event;
    }

    public void Delete(Event @event)
    {
        _context.Events.Remove(@event);
        _context.SaveChanges();
    }
}
