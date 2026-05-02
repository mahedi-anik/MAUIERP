using MAUIERP.Domain.Entities.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAUIERP.Infrastructure.Data.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.TaxNumber)
                .HasMaxLength(50);

            builder.Property(c => c.Phone)
                .HasMaxLength(20);

            builder.Property(c => c.Email)
                .HasMaxLength(100);

            builder.Property(c => c.Website)
                .HasMaxLength(200);

            builder.Property(c => c.Address)
                .HasMaxLength(500);

            builder.Property(c => c.City)
                .HasMaxLength(100);

            builder.Property(c => c.State)
                .HasMaxLength(100);

            builder.Property(c => c.Country)
                .HasMaxLength(100);

            builder.Property(c => c.PostalCode)
                .HasMaxLength(20);

            builder.HasIndex(c => c.Code)
                .IsUnique();

            builder.HasIndex(c => c.TaxNumber)
                .IsUnique()
                .HasFilter("[TaxNumber] IS NOT NULL");

            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
