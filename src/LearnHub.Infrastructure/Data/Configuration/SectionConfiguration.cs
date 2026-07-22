using LearnHub.Domain.Courses.Sections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class SectionConfiguration
    : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");

        builder.HasKey(x => x.Id);



        // Properties

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);


        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000);


        builder.Property(x => x.Order)
            .IsRequired();



        // Course Relationship

        builder.HasOne(x => x.Course)
            .WithMany(x => x.Sections)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);



        // Lessons Relationship

        builder.HasMany(x => x.Lessons)
            .WithOne(x => x.Section)
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Cascade);



        // Prevent duplicate section ordering inside same course

        builder.HasIndex(x => new
        {
            x.CourseId,
            x.Order
        })
        .IsUnique();



        // Domain Events

        builder.Ignore(x => x.DomainEvents);
    }
}

