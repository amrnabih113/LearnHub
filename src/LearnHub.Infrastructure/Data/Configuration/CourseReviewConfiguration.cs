using LearnHub.Domain.Reviews.CourseReviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class CourseReviewConfiguration : IEntityTypeConfiguration<CourseReview>
{
    public void Configure(EntityTypeBuilder<CourseReview> builder)
    {
        builder.ToTable("CourseReviews");

        builder.HasKey(x => x.Id);


        builder.Property(x => x.CourseId)
            .IsRequired();


        builder.Property(x => x.StudentId)
            .IsRequired();


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
            x.CourseId,
            x.StudentId
        })
        .IsUnique();
    }
}