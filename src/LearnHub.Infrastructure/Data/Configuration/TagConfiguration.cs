using LearnHub.Domain.Classification.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;


public sealed class TagConfiguration
    : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(x => x.Id);


        // Properties

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);


        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(100);


        builder.Property(x => x.Description)
            .HasMaxLength(500);



        // Unique Constraints

        builder.HasIndex(x => x.Name)
            .IsUnique();


        builder.HasIndex(x => x.Slug)
            .IsUnique();



        // Relationship with CourseTag

        builder.HasMany(x => x.CourseTags)
            .WithOne(x => x.Tag)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);



        // Domain Events

        builder.Ignore(x => x.DomainEvents);
    }
}