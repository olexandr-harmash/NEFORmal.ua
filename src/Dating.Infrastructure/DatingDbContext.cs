using Microsoft.EntityFrameworkCore;

using NEFORmal.ua.Dating.ApplicationCore.Models;
using NEFORmal.ua.Dating.Infrastructure.EntityTypeConfiguration;

namespace NEFORmal.ua.Dating.Infrastructure;

public class DatingDbContext : DbContext
{
    public DbSet<Date>    Dates    { get; init; }
    public DbSet<Profile> Profiles { get; init; }

    public DatingDbContext(DbContextOptions<DatingDbContext> options) : base(options) {}

      protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new DateEntityTypeConfiguration   ());
        modelBuilder.ApplyConfiguration(new ProfileEntityTypeConfiguration());
    }
}