// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Popsalon.Domain.Entities;

namespace Popsalon.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.Date)
            .IsRequired()
            .HasColumnName("date");

        builder.Property(a => a.Notes)
            .HasMaxLength(500)
            .HasColumnName("notes");

        builder.Property(a => a.CustomerId)
            .IsRequired()
            .HasColumnName("customer_id");

        builder.Property(a => a.CreatedAt).HasColumnName("created_at");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(a => a.Customer)
            .WithMany(c => c.Appointments)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
