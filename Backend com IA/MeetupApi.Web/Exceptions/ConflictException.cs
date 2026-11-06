namespace MeetupApi.Web.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
