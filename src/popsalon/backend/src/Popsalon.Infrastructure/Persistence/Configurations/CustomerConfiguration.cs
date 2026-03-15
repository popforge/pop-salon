// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Popsalon.Domain.Entities;

namespace Popsalon.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100).HasColumnName("first_name");
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100).HasColumnName("last_name");
        builder.Property(c => c.Email).HasMaxLength(255).HasColumnName("email");
        builder.Property(c => c.Phone).HasMaxLength(20).HasColumnName("phone");

        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(c => c.Email).IsUnique().HasFilter("email IS NOT NULL");
    }
}
