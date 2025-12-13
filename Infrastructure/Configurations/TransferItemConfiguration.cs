using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;

public class TransferItemConfiguration : IEntityTypeConfiguration<TransferItem>
{
    public void Configure(EntityTypeBuilder<TransferItem> builder)
    {
        builder.Property(i => i.TrackName)
            .HasMaxLength(200);

        builder.Property(i => i.Artist)
            .HasMaxLength(200);

        builder.Property(i => i.Album)
            .HasMaxLength(200);

        builder.HasOne(i => i.TransferOperation)
            .WithMany()
            .HasForeignKey(i => i.TransferOperationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
