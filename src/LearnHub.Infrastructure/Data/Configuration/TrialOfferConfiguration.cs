using LearnHub.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class TrialOfferConfiguration
    : IEntityTypeConfiguration<TrialOffer>
{
    public void Configure(EntityTypeBuilder<TrialOffer> builder)
    {
        builder.ToTable("TrialOffers");


        builder.HasKey(x => x.Id);


        builder.Property(x => x.StudentId)
            .IsRequired();


        builder.Property(x => x.Tier)
            .HasConversion<string>()
            .HasMaxLength(30);


        builder.Property(x => x.DurationDays)
            .IsRequired();


        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();


        builder.HasIndex(x => x.StudentId)
            .IsUnique();
    }
}