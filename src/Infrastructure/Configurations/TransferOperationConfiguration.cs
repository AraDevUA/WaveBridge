using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;

public class TransferOperationConfiguration : IEntityTypeConfiguration<Entities.TransferOperation>
{
    public void Configure(EntityTypeBuilder<Entities.TransferOperation> builder)
    {
        builder.Property(t => t.Status)
            .IsRequired();

        builder.Property(t => t.MergedTargetPlaylistId)
            .HasMaxLength(200);

        builder.Property(t => t.MergedTargetPlaylistUrl)
            .HasMaxLength(500);

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.TransferPlaylists)
            .WithOne(t => t.TransferOperation)
            .HasForeignKey(t => t.TransferOperationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
