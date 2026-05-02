using MAUIERP.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAUIERP.Infrastructure.Data.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermissions");

            // Define composite primary key
            builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // Configure relationships
            builder.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure properties
            builder.Property(rp => rp.CanRead)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(rp => rp.CanCreate)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(rp => rp.CanUpdate)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(rp => rp.CanDelete)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
