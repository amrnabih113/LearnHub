using LearnHub.Domain.Assessments;
using LearnHub.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("Quizzes");

        builder.HasKey(x => x.Id);


        // Basic properties
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);


        // Course relationship
        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Section relationship
        builder.HasOne<LearnHub.Domain.Courses.Sections.Section>()
            .WithMany()
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // Enums
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => new { x.CourseId, x.SectionId, x.Type });



        // PassingPolicy Value Object
        builder.OwnsOne(x => x.PassingPolicy, policy =>
        {
            policy.Property(x => x.MaxAttempts)
                .HasColumnName("MaxAttempts")
                .IsRequired();

            policy.Property(x => x.PassPercentage)
                .HasColumnName("PassPercentage")
                .IsRequired();
        });



        // Questions
        builder.HasMany(x => x.Questions)
            .WithOne()
            .HasForeignKey("QuizId")
            .OnDelete(DeleteBehavior.Cascade);



        // Ignore domain events
        builder.Ignore(x => x.DomainEvents);
    }
}
;
