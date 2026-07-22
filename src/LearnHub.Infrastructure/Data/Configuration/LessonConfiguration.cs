using LearnHub.Domain.Courses.Sections.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;


public sealed class LessonConfiguration
    : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons");

        builder.HasKey(x => x.Id);



        // Properties

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);


        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000);


        builder.Property(x => x.VideoUrl)
            .IsRequired()
            .HasMaxLength(500);


        builder.Property(x => x.Content)
            .IsRequired();


        builder.Property(x => x.DurationInMinutes)
            .IsRequired();


        builder.Property(x => x.Order)
            .IsRequired();



        builder.Property(x => x.IsPreview)
            .IsRequired();



        // Section Relationship

        builder.HasOne(x => x.Section)
            .WithMany(x => x.Lessons)
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Cascade);



        // Resources Relationship

        builder.HasMany(x => x.Resources)
            .WithOne(x => x.Lesson)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);



        // Ordering inside Section

        builder.HasIndex(x => new
        {
            x.SectionId,
            x.Order
        })
        .IsUnique();



        // Ignore domain events

        builder.Ignore(x => x.DomainEvents);
    }
}