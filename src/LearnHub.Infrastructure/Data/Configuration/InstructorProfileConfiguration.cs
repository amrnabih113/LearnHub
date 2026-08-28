using LearnHub.Domain.Instructor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class InstructorProfileConfiguration : IEntityTypeConfiguration<InstructorProfile>
{
    public void Configure(EntityTypeBuilder<InstructorProfile> builder)
    {
        builder.ToTable("InstructorProfiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProfessionalTitle)
            .HasMaxLength(200);

        builder.Property(x => x.Headline)
            .HasMaxLength(300);

        builder.Property(x => x.Biography)
            .HasMaxLength(4000);

        builder.Property(x => x.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(x => x.VerificationStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(1000);

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<InstructorProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Experiences)
            .WithOne()
            .HasForeignKey(x => x.InstructorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Education)
            .WithOne()
            .HasForeignKey(x => x.InstructorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Certifications)
            .WithOne()
            .HasForeignKey(x => x.InstructorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Skills)
            .WithOne()
            .HasForeignKey(x => x.InstructorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Languages)
            .WithOne()
            .HasForeignKey(x => x.InstructorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Links)
            .WithOne()
            .HasForeignKey(x => x.InstructorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(x => x.DomainEvents);
    }
}
