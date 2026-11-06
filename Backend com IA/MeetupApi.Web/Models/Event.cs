namespace MeetupApi.Web.Models;

public class Event
{
    public long Id { get; private set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public List<Registration> Registrations { get; } = new();

    public Event() { }

    public Event(string title, string description, DateTime dateTime, string location, int capacity)
    {
        Title = title;
        Description = description;
        DateTime = dateTime;
        Location = location;
        Capacity = capacity;
    }

    public override bool Equals(object? obj) =>
        obj is Event other && Id != 0 && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
