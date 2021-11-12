using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var meetups = new List<Meetup>();

// Crud - Create
app.MapPost("/meetups", ([FromBody] Meetup newMeetup) =>
{
    newMeetup.Id = Guid.NewGuid();
    meetups.Add(newMeetup);

    return Results.Created($"/meetups/{newMeetup.Id}", newMeetup);
});

// cRud - read
app.MapGet("/meetups", () => Results.Ok(meetups));

// crUd - update
app.MapPut("/meetups/{id:guid}", ([FromRoute] Guid id, [FromBody] Meetup updatedMeetup) =>
{
    var oldMeetup = meetups.SingleOrDefault(meetup => meetup.Id == id);

    // meetup with provided id does not exist
    if (oldMeetup is null)
    {
        return Results.NotFound();
    }

    oldMeetup.Topic = updatedMeetup.Topic;
    oldMeetup.Place = updatedMeetup.Place;
    oldMeetup.Duration = updatedMeetup.Duration;

    return Results.NoContent();
});

// cruD - delete
app.MapDelete("/meetups/{id:guid}", ([FromRoute] Guid id) =>
{
    var meetupToDelete = meetups.SingleOrDefault(meetup => meetup.Id == id);

    // meetup with provided id does not exist
    if (meetupToDelete is null)
    {
        return Results.NotFound();
    }

    meetups.Remove(meetupToDelete);
    return Results.Ok(meetupToDelete);
});

app.Run();

class Meetup
{
    public Guid? Id { get; set; }
    public string Topic { get; set; }
    public string Place { get; set; }
    public int Duration { get; set; }
}
