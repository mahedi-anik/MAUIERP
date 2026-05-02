using MAUIERP.Domain.Entities.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAUIERP.Infrastructure.Data.Configurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branches");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.Phone)
                .HasMaxLength(20);

            builder.Property(b => b.Email)
                .HasMaxLength(100);

            builder.HasIndex(b => new { b.CompanyId, b.Code })
                .IsUnique();

            builder.HasOne(b => b.Company)
                .WithMany(c => c.Branches)
                .HasForeignKey(b => b.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(b => !b.IsDeleted);
        }
    }

}
