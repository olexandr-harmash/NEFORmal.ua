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
            var profile1 = new Profile
                (
                    "SID001",
                    "John Doe",
                    "A software developer who loves coding.",
                    30,
                    "Male"
                );

            profile1.UpdateProfilePhotos(new List<string> { "photo1.jpg", "photo2.jpg" });

            var profile2 = new Profile
                (
                    "SID002",
                    "Jane Smith",
                    "An artist who enjoys painting and photography.",
                    28,
                    "Female"
                );
                
            profile2.UpdateProfilePhotos(new List<string> { "photo1.jpg", "photo2.jpg" });

            var profiles = new List<Profile>{ profile1, profile2 };

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
