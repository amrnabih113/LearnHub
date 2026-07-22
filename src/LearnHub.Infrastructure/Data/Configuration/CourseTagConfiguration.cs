using LearnHub.Domain.Classification.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class CourseTagConfiguration
    : IEntityTypeConfiguration<CourseTag>
{
    public void Configure(EntityTypeBuilder<CourseTag> builder)
    {
        builder.ToTable("CourseTags");

        builder.HasKey(x => new
        {
            x.CourseId,
            x.TagId
        });


        builder.HasOne(x => x.Course)
            .WithMany(x => x.CourseTags)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(x => x.Tag)
            .WithMany(x => x.CourseTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
