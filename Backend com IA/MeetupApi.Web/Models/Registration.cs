namespace MeetupApi.Web.Models;

public class Registration
{
    public long Id { get; private set; }
    public long EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public long ParticipantId { get; private set; }
    public Participant Participant { get; private set; } = null!;
    public DateTime RegisteredAt { get; private set; }
    public RegistrationStatus Status { get; set; }

    public Registration() { }

    public Registration(Event @event, Participant participant, DateTime registeredAt, RegistrationStatus status)
    {
        Event = @event;
        Participant = participant;
        RegisteredAt = registeredAt;
        Status = status;
    }

    public override bool Equals(object? obj) =>
        obj is Registration other && Id != 0 && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
