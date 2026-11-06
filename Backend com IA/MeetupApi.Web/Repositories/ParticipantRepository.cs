using MeetupApi.Web.Data;
using MeetupApi.Web.Models;

namespace MeetupApi.Web.Repositories;

public class ParticipantRepository : IParticipantRepository
{
    private readonly MeetupDbContext _context;

    public ParticipantRepository(MeetupDbContext context)
    {
        _context = context;
    }

    public Participant? FindById(long id) =>
        _context.Participants.FirstOrDefault(p => p.Id == id);

    public List<Participant> FindAll() =>
        _context.Participants.ToList();

    public Participant? FindByEmail(string email) =>
        _context.Participants.FirstOrDefault(p => p.Email == email);

    public Participant Save(Participant participant)
    {
        if (participant.Id == 0)
        {
            _context.Participants.Add(participant);
        }
        _context.SaveChanges();
        return participant;
    }

    public void Delete(Participant participant)
    {
        _context.Participants.Remove(participant);
        _context.SaveChanges();
    }
}
