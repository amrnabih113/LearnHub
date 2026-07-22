using LearnHub.Domain.Enrollments;
using LearnHub.Domain.Enrollments.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class EnrollmentConfiguration
    : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.HasKey(x => x.Id);


        // Student
        builder.Property(x => x.StudentId)
            .IsRequired();


        // Course
        builder.Property(x => x.CourseId)
            .IsRequired();



        // Status Enum
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();



        builder.Property(x => x.ProgressPercentage)
            .HasPrecision(5, 2)
            .IsRequired();



        builder.Property(x => x.CompletedAtUtc);



        // Lesson Progress
        builder.HasMany(x => x.LessonsProgress)
            .WithOne()
            .HasForeignKey(x => x.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);



        // Certificate One-to-One
        builder.HasOne(x => x.Certificate)
            .WithOne(x => x.Enrollment)
            .HasForeignKey<Certificate>(x => x.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);



        builder.Ignore(x => x.DomainEvents);
    }
}
