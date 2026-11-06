using MeetupApi.Web.Data;
using MeetupApi.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetupApi.Web.Repositories;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly MeetupDbContext _context;

    public RegistrationRepository(MeetupDbContext context)
    {
        _context = context;
    }

    public Registration? FindById(long id) =>
        _context.Registrations
            .Include(r => r.Event)
            .Include(r => r.Participant)
            .FirstOrDefault(r => r.Id == id);

    public bool ExistsByEventAndParticipantAndStatus(Event @event, Participant participant, RegistrationStatus status) =>
        _context.Registrations.Any(r =>
            r.EventId == @event.Id && r.ParticipantId == participant.Id && r.Status == status);

    public long CountByEventAndStatus(Event @event, RegistrationStatus status) =>
        _context.Registrations.LongCount(r => r.EventId == @event.Id && r.Status == status);

    public List<Registration> FindByEventId(long eventId) =>
        _context.Registrations
            .Include(r => r.Event)
            .Include(r => r.Participant)
            .Where(r => r.EventId == eventId)
            .ToList();

    public Registration Save(Registration registration)
    {
        if (registration.Id == 0)
        {
            _context.Registrations.Add(registration);
        }
        _context.SaveChanges();
        return registration;
    }
}
