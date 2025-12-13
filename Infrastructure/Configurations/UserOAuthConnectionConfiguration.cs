using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Domain.Entities.Configurations;

public class UserOAuthConnectionConfiguration : IEntityTypeConfiguration<UserOAuthConnection>
{
    public void Configure(EntityTypeBuilder<UserOAuthConnection> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.User)
            .WithMany() 
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.ProviderUserId)
            .IsRequired();

        builder.Property(c => c.AccessToken)
            .HasMaxLength(2000);

        builder.Property(c => c.RefreshToken)
            .HasMaxLength(2000);

        builder.Property(c => c.AccessTokenExpiresAtUtc)
            .IsRequired();
    }
}
