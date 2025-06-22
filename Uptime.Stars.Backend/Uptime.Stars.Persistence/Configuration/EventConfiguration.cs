using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;

namespace Uptime.Stars.Persistence.Configuration;
internal sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.IsUp)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(entity => entity.IsImportant)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(entity => entity.TimestampUtc)
            .IsRequired();

        builder.Property(entity => entity.Message)
            .HasMaxLength(200);

        builder.Property(entity => entity.FalsePositive)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(entity => entity.Category)
            .HasConversion(property => (int?)property, value => (Category?)value)
            .HasMaxLength(50);

        builder.Property(entity => entity.Note)
            .HasMaxLength(200);

        builder.Property(entity => entity.TicketId)
            .HasMaxLength(20);

        builder.Property(entity => entity.MaintenanceType)
            .HasConversion(property => (int?)property, value => (MaintenanceType?)value);

        builder.HasOne(entity => entity.Monitor)
            .WithMany(group => group.Events)
            .HasForeignKey(entity => entity.MonitorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}