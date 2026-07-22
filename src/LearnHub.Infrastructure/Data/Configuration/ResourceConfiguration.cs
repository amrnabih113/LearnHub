using LearnHub.Domain.Courses.Sections.Lessons.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;


public sealed class ResourceConfiguration
    : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("Resources");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);


        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(500);



        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();



        builder.Property(x => x.SizeInBytes)
            .IsRequired();



        // Lesson relationship

        builder.HasOne(x => x.Lesson)
            .WithMany(x => x.Resources)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);



        builder.Ignore(x => x.DomainEvents);
    }
}