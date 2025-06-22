using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;

namespace Uptime.Stars.Persistence.Configuration;
internal sealed class MonitorConfiguration : IEntityTypeConfiguration<ComponentMonitor>
{
    public void Configure(EntityTypeBuilder<ComponentMonitor> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.Description)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(entity => entity.Group)
            .WithMany(group => group.Components)
            .HasForeignKey(entity => entity.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(entity => entity.Type)
            .HasConversion(property => (int) property, value => (MonitorType) value)
            .IsRequired();

        builder.Property(entity => entity.Target)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(entity => entity.IntervalInMinutes)
            .HasDefaultValue(1)
            .IsRequired();

        builder.Property(entity => entity.TiemoutInMilliseconds)
            .HasDefaultValue(10000)
            .IsRequired();

        builder.Property(entity => entity.RequestHeaders)
            .HasConversion(property => string.Join(",", property), value => value.Split(",", StringSplitOptions.RemoveEmptyEntries))
            .HasMaxLength(200);

        builder.Property(entity => entity.SearchMode)
            .HasConversion(property => (int?)property, value => (TextSearchMode?)value);

        builder.Property(entity => entity.ExpectedText)
            .HasMaxLength(100);

        builder.Property(entity => entity.AlertEmails)
            .HasConversion(property => string.Join(",", property), value => value.Split(",", StringSplitOptions.RemoveEmptyEntries))
            .HasMaxLength(200);

        builder.Property(entity => entity.AlertMessage)
            .HasMaxLength(1000);

        builder.Property(entity => entity.AlertDelayMinutes)
            .HasDefaultValue(1)
            .IsRequired();

        builder.Property(entity => entity.AlertResendCycles)
            .HasDefaultValue(1)
            .IsRequired();

        builder.Property(entity => entity.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(entity => entity.CreatedAt)
            .IsRequired();

        builder.Property(entity => entity.IsUp);
    }
}