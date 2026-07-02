using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;

public class TransferTrackConfiguration : IEntityTypeConfiguration<TransferTrack>
{
    public void Configure(EntityTypeBuilder<TransferTrack> builder)
    {
        builder.Property(i => i.SourceId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.TargetId)
            .HasMaxLength(200);

        builder.Property(i => i.TrackName)
            .HasMaxLength(200);

        builder.Property(i => i.Artist)
            .HasMaxLength(200);

        builder.Property(i => i.Album)
            .HasMaxLength(200);

        builder.Property(i => i.ArtworkUrl)
            .HasMaxLength(500);

        builder.Property(i => i.ErrorMessage)
            .HasMaxLength(500);

        builder.HasOne(i => i.TransferPlaylist)
            .WithMany(t => t.TransferTracks)
            .HasForeignKey(i => i.TransferPlaylistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
