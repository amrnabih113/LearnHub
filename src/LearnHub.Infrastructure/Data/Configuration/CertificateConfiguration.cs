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

        builder.HasIndex(x => x.StudentId);

        builder.Property(x => x.CourseId)
            .IsRequired();

        builder.HasIndex(x => x.CourseId);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.StudentName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CourseName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.InstructorName)
            .HasMaxLength(200);

        builder.Property(x => x.PdfUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.IssuedAtUtc)
            .IsRequired();

        builder.Property(x => x.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.RevokedAtUtc);

        builder.Property(x => x.RevocationReason)
            .HasMaxLength(500);

        builder.Ignore(x => x.DomainEvents);
    }
}