namespace Meets.WebApi;

using Microsoft.EntityFrameworkCore;
using Meets.WebApi.Meetup;

public class DatabaseContext : DbContext
{
    public DbSet<MeetupEntity> Meetups { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }
}
