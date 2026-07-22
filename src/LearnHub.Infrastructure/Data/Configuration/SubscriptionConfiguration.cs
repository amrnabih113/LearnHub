using LearnHub.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(x => x.Id);


        builder.Property(x => x.StudentId)
            .IsRequired();


        builder.Property(x => x.Tier)
            .HasConversion<string>()
            .HasMaxLength(30);


        builder.Property(x => x.BillingCycle)
            .HasConversion<string>()
            .HasMaxLength(30);


        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);


        builder.Property(x => x.AutoRenewEnabled)
            .IsRequired();


        builder.HasMany(x => x.Payments)
            .WithOne(x => x.Subscription)
            .HasForeignKey(x => x.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Navigation(x => x.Payments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);


        builder.HasIndex(x => new
        {
            x.StudentId,
            x.Status
        });
    }
}