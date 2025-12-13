using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;

public class TransferOperationConfiguration : IEntityTypeConfiguration<Entities.TransferOperation>
{
    public void Configure(EntityTypeBuilder<Entities.TransferOperation> builder)
    {
        builder.Property(t => t.Status)
            .IsRequired();

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
