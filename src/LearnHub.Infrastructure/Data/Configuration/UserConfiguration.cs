using LearnHub.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(2048);

        builder.Property(x => x.Bio)
            .HasMaxLength(1000);

        builder.Property(x => x.Country)
            .HasMaxLength(100);

        builder.Property(x => x.DateOfBirth);

        builder.Property(x => x.IsEmailVerified)
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        builder.Ignore(x => x.DomainEvents);

        builder.HasMany(x => x.Roles)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
