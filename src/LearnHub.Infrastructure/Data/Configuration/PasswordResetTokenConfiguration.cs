using LearnHub.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("PasswordResetTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.ExpiresOnUtc)
            .IsRequired();

        builder.Property(x => x.UsedAtUtc);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        builder.Ignore(x => x.DomainEvents);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.HasIndex(x => x.UserId);
    }
}