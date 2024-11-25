using Microsoft.EntityFrameworkCore;

using NEFORmal.ua.Dating.Infrastructure;
using NEFORmal.ua.Dating.ApplicationCore.Models;

public class DatingDbSeed
{
    private readonly DatingDbContext _context;

    public DatingDbSeed(DatingDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await _context.Database.MigrateAsync();

        if (!_context.Set<Profile>().Any())
        {
            var profiles = new List<Profile>
            { 
                new Profile
                (
                    "SID001",
                    "John Doe",
                    "A software developer who loves coding.",
                    30,
                    "Male",
                    new List<string> { "photo1.jpg", "photo2.jpg" }
                ),

                new Profile
                (
                    "SID002",
                    "Jane Smith",
                    "An artist who enjoys painting and photography.",
                    28,
                    "Female",
                    new List<string> { "photo1.jpg", "photo2.jpg" }
                )
            };

            _context.Set<Profile>().AddRange(profiles);
            await _context.SaveChangesAsync();
        }

        if (!_context.Set<Date>().Any())
        {
            var dateRecords = new List<Date>
            {
                new Date
                (
                    1,
                    2,
                    false,
                    "Looking forward to our date!"
                )
            };

            _context.Set<Date>().AddRange(dateRecords);
            await _context.SaveChangesAsync();
        }
    }
}
