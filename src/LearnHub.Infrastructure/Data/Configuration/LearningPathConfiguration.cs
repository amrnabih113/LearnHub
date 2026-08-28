using LearnHub.Domain.LearningPaths;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class LearningPathConfiguration : IEntityTypeConfiguration<LearningPath>
{
    public void Configure(EntityTypeBuilder<LearningPath> builder)
    {
        builder.ToTable("LearningPaths");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(3000);

        builder.Property(x => x.ShortDescription)
            .HasMaxLength(500);

        builder.Property(x => x.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Level)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Level);
        builder.HasIndex(x => x.OwnerId);

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Courses)
            .WithOne(x => x.LearningPath)
            .HasForeignKey(x => x.LearningPathId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(x => x.DomainEvents);
    }
}
