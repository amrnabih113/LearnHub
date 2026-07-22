using LearnHub.Domain.Reviews.InstructorReviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class InstructorReviewConfiguration : IEntityTypeConfiguration<InstructorReview>
{
    public void Configure(EntityTypeBuilder<InstructorReview> builder)
    {
        builder.ToTable("InstructorReviews");

        builder.HasKey(x => x.Id);


        builder.Property(x => x.InstructorId)
            .IsRequired();


        builder.Property(x => x.StudentId)
            .IsRequired();


        builder.Property(x => x.CourseId);


        builder.Property(x => x.Comment)
            .HasMaxLength(2000)
            .IsRequired();


        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();


        builder.OwnsOne(x => x.Rating, rating =>
        {
            rating.Property(x => x.Value)
                .HasColumnName("RatingValue")
                .IsRequired();
        });


        builder.HasIndex(x => new
        {
            x.InstructorId,
            x.StudentId,
            x.CourseId
        })
        .IsUnique();
    }
}