using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Persistence.Configuration;
internal sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.Description)
            .HasMaxLength(200)
            .IsRequired();
    }
}
