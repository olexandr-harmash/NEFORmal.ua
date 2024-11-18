using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NEFORmal.ua.Dating.ApplicationCore.Models;

namespace NEFORmal.ua.Dating.Infrastructure.EntityTypeConfiguration;

public class DateEntityTypeConfiguration  : IEntityTypeConfiguration<Date>
{
    public void Configure(EntityTypeBuilder<Date> builder) 
    {
        builder.HasKey(d => new { d.SenderId, d.ReceiverId });

        builder.Property(d => d.Message)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(d => d.IsApproved)
            .IsRequired();
        
        builder.HasIndex(p => p.SenderId)
            .HasDatabaseName("IX_Date_SenderId");

        builder.HasIndex(p => p.ReceiverId)
            .HasDatabaseName("IX_Date_ReceiverId");
        
        // Внешний ключ для SenderId
        builder.HasOne(d => d.Sender) // Навигационное свойство, которое связывает Date с Profile (Sender)
            .WithMany() // Для получателя можно использовать WithMany(), если связь не является обратной
            .HasForeignKey(d => d.SenderId)
            .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление

        // Внешний ключ для ReceiverId
        builder.HasOne(d => d.Receiver) // Навигационное свойство, которое связывает Date с Profile (Receiver)
            .WithMany() // Для получателя тоже можно использовать WithMany()
            .HasForeignKey(d => d.ReceiverId)
            .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление
    }
}