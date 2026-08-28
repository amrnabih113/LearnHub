using LearnHub.Domain.LearningPaths;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class LearningPathCourseConfiguration : IEntityTypeConfiguration<LearningPathCourse>
{
    public void Configure(EntityTypeBuilder<LearningPathCourse> builder)
    {
        builder.ToTable("LearningPathCourses");

        // Composite PK
        builder.HasKey(x => new { x.LearningPathId, x.CourseId });

        builder.Property(x => x.Order)
            .IsRequired();

        builder.Property(x => x.IsRequired)
            .IsRequired()
            .HasDefaultValue(true);

        // Unique index on LearningPathId + Order
        builder.HasIndex(x => new { x.LearningPathId, x.Order })
            .IsUnique();

        builder.HasOne(x => x.Course)
            .WithMany()
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
