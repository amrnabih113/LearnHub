using LearnHub.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class CourseConfiguration
    : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(x => x.Id);


        // Basic Properties

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);


        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);


        builder.Property(x => x.ThumbnailUrl)
            .HasMaxLength(500);


        builder.Property(x => x.Country)
            .HasMaxLength(100);



        // Instructor Relationship

        builder.HasOne(x => x.Instructor)
            .WithMany()
            .HasForeignKey(x => x.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);



        // Category Relationship

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Courses)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);



        // Money Value Object

        builder.OwnsOne(x => x.Price, price =>
        {
            price.Property(x => x.Amount)
                .HasColumnName("Price")
                .HasPrecision(18, 2)
                .IsRequired();


            price.Property(x => x.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });



        // Language Value Object

        builder.OwnsOne(x => x.Language, language =>
        {
            language.Property(x => x.Code)
                .HasColumnName("LanguageCode")
                .HasMaxLength(5)
                .IsRequired();
        });



        // Course Tags

        builder.HasMany(x => x.CourseTags)
            .WithOne(x => x.Course)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);



        // Course Sections

        builder.HasMany(x => x.Sections)
            .WithOne()
            .HasForeignKey("CourseId")
            .OnDelete(DeleteBehavior.Cascade);



        // Enum Conversions

        builder.Property(x => x.Level)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();


        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();


        builder.Property(x => x.RequiredSubscriptionTier)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();



        builder.Property(x => x.IsIncludedInSubscription)
            .IsRequired();

        // Search Indexes
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => x.InstructorId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Level);
        builder.HasIndex(x => x.CreatedAtUtc);
        builder.HasIndex(x => new { x.Status, x.CategoryId });
        builder.HasIndex(x => new { x.Status, x.Level });

        // Ignore domain-only properties

        builder.Ignore(x => x.DomainEvents);
    }
}
