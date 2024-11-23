using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NEFORmal.ua.Dating.ApplicationCore.Models;

namespace NEFORmal.ua.Dating.Infrastructure.EntityTypeConfiguration;

public class ProfileEntityTypeConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Age)
            .IsRequired();

        builder.Property(p => p.Bio)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(p => p.Sex)
            .IsRequired();

        builder.Property(p => p.Sid)
            .IsRequired();

        builder.Property(p => p.ProfilePhotos)
            .IsRequired();

        builder.HasIndex(p => new { p.Sex, p.Age })
            .HasDatabaseName("IX_Profile_Sex_Age");
    }
}
