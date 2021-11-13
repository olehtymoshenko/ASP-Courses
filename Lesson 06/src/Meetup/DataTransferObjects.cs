namespace Meets.WebApi.Meetup;

public class ReadMeetupDto
{
    public Guid Id { get; set; }
    public string Topic { get; set; }
    public string Place { get; set; }
    public int Duration { get; set; }
}

public class CreateMeetupDto
{
    public string Topic { get; set; }
    public string Place { get; set; }
    public int Duration { get; set; }
}

public class UpdateMeetupDto
{
    public string Topic { get; set; }
    public string Place { get; set; }
    public int Duration { get; set; }
}
