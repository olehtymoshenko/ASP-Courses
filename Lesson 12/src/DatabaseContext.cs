namespace Meets.WebApi;

using Microsoft.EntityFrameworkCore;
using Meets.WebApi.Meetup;
using Meets.WebApi.User;

public class DatabaseContext : DbContext
{
    public DbSet<MeetupEntity> Meetups { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }
}
