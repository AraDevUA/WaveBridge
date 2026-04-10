using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;

public class TransferPlaylistConfiguration : IEntityTypeConfiguration<TransferPlaylist>
{
    public void Configure(EntityTypeBuilder<TransferPlaylist> builder)
    {
        builder.Property(t => t.SourcePlaylistId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.TargetPlaylistId)
            .HasMaxLength(200);

        builder.Property(t => t.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.ArtworkUrl)
            .HasMaxLength(500);

        builder.Property(t => t.TargetPlaylistUrl)
            .HasMaxLength(500);

        builder.HasMany(t => t.TransferTracks)
            .WithOne(t => t.TransferPlaylist)
            .HasForeignKey(t => t.TransferPlaylistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
