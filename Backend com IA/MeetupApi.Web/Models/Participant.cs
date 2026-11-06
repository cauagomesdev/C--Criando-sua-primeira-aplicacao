namespace MeetupApi.Web.Models;

public class Participant
{
    public long Id { get; private set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Participant() { }

    public Participant(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public override bool Equals(object? obj) =>
        obj is Participant other && Id != 0 && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
