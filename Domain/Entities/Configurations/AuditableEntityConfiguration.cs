using Domain.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations;

public class AuditableEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity> where TEntity : class, IEntity<TKey>, IAuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedUtc)
            .HasDefaultValueSql("timezone('utc', now())")
            .ValueGeneratedOnAdd();
    }
}
