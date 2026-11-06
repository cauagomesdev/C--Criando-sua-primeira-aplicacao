using MeetupApi.Web.Data;
using MeetupApi.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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
            .Include(registration => registration.Event)
            .Include(registration => registration.Participant)
            .FirstOrDefault(registration => registration.Id == id);

    public bool ExistsByEventAndParticipantAndStatus(Event @event, Participant participant, RegistrationStatus status) =>
        _context.Registrations.Any(registration =>
            registration.EventId == @event.Id && registration.ParticipantId == participant.Id && registration.Status == status);

    public long CountByEventAndStatus(Event @event, RegistrationStatus status) =>
        _context.Registrations.LongCount(registration => registration.EventId == @event.Id && registration.Status == status);

    public List<Registration> FindByEventId(long eventId) =>
        _context.Registrations
            .Include(registration => registration.Event)
            .Include(registration => registration.Participant)
            .Where(registration => registration.EventId == eventId)
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

    public Registration ExecuteWithEventLock(long eventId, Func<Registration> action)
    {
        using IDbContextTransaction transaction = _context.Database.BeginTransaction();

        // UPDATE que não altera nada: o banco trava a linha do evento até o commit/rollback
        _context.Events
            .Where(@event => @event.Id == eventId)
            .ExecuteUpdate(setters => setters.SetProperty(@event => @event.Capacity, @event => @event.Capacity));

        Registration result = action();
        transaction.Commit();
        return result;
    }
}
