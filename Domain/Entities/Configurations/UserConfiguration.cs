using Domain.Entities.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.Metrics;

namespace Domain.Entities.Configuration;

public class UserConfiguration : AuditableEntityConfiguration<User, Guid>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        
    }
}
