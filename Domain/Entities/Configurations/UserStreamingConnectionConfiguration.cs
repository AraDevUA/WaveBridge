using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;

public class UserStreamingConnectionConfiguration : IEntityTypeConfiguration<UserStreamingConnection>
{
    public void Configure(EntityTypeBuilder<UserStreamingConnection> builder)
    {
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.Service)
            .IsRequired();

        builder.Property(c => c.ExternalUserId)
            .IsRequired();

        builder.Property(c => c.AccessToken)
            .HasMaxLength(2000);

        builder.Property(c => c.RefreshToken)
            .HasMaxLength(2000);

        builder.Property(c => c.AccessTokenExpiresAtUtc)
            .IsRequired();
    }
}
