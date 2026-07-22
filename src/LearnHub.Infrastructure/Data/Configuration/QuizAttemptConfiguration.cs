using LearnHub.Domain.Assessments;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class QuizAttemptConfiguration
    : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.ToTable("QuizAttempts");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50);



        builder.HasOne<Quiz>()
            .WithMany()
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.Cascade);



        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);



        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);



        builder.HasMany(x => x.Answers)
            .WithOne()
            .HasForeignKey("QuizAttemptId")
            .OnDelete(DeleteBehavior.Cascade);



        builder.OwnsOne(x => x.Grade, grade =>
  {
      grade.Property(g => g.Score)
          .HasColumnName("GradeScore");

      grade.Property(g => g.TotalScore)
          .HasColumnName("GradeTotalScore");

      grade.Property(g => g.ScorePercentage)
          .HasColumnName("GradePercentage");

      grade.Property(g => g.IsPassed)
          .HasColumnName("GradePassed");
  });

        builder.Ignore(x => x.DomainEvents);
    }
}
