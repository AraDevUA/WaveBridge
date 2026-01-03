using Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;

public class RoleEntityConfiguration : AuditableEntityConfiguration<Role, Guid>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);
    }
}
