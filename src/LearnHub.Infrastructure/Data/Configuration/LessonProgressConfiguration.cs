using LearnHub.Domain.Enrollments.LessonProgress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class LessonProgressConfiguration
    : IEntityTypeConfiguration<LessonProgress>
{
    public void Configure(EntityTypeBuilder<LessonProgress> builder)
    {
        builder.ToTable("LessonProgress");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.EnrollmentId)
            .IsRequired();


        builder.Property(x => x.LessonId)
            .IsRequired();



        builder.Property(x => x.IsCompleted)
            .IsRequired();



        builder.Property(x => x.WatchDurationSeconds)
            .IsRequired();



        builder.Property(x => x.CompletedAtUtc);



        // Prevent duplicate progress records
        builder.HasIndex(x => new
        {
            x.EnrollmentId,
            x.LessonId
        })
        .IsUnique();



        builder.Ignore(x => x.DomainEvents);
    }
}