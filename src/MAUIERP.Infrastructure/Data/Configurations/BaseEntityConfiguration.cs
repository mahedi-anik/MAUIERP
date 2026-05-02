using MAUIERP.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAUIERP.Infrastructure.Data.Configurations
{
    public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .IsRequired()
                .HasDefaultValueSql("NEWID()");

            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .IsRequired(false);

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.DeletedAt)
                .IsRequired(false);

            builder.HasIndex(e => e.IsDeleted);
        }
    }
}
