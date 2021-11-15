namespace Meets.WebApi.Meetup;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MeetupEntity
{
    public Guid Id { get; set; }
    public string Topic { get; set; }
    public string Place { get; set; }
    public int Duration { get; set; }
}

internal class MeetupEntityTypeConfiguration : IEntityTypeConfiguration<MeetupEntity>
{
    public void Configure(EntityTypeBuilder<MeetupEntity> entity)
    {
        entity.ToTable("meetups");

        entity
            .HasKey(meetup => meetup.Id)
            .HasName("pk_meetups");

        entity
            .Property(meetup => meetup.Id)
            .HasColumnName("id");

        entity
            .Property(meetup => meetup.Topic)
            .HasColumnName("topic");

        entity
            .Property(meetup => meetup.Place)
            .HasColumnName("place");

        entity
            .Property(meetup => meetup.Duration)
            .HasColumnName("duration");
    }
}
