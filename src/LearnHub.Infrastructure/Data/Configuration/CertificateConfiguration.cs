using LearnHub.Domain.Enrollments.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class CertificateConfiguration
    : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("Certificates");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.EnrollmentId)
            .IsRequired();



        builder.Property(x => x.StudentId)
            .IsRequired();



        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(100);



        builder.Property(x => x.IssuedAtUtc)
            .IsRequired();



        // Unique certificate code
        builder.HasIndex(x => x.Code)
            .IsUnique();



        builder.Ignore(x => x.DomainEvents);
    }
}