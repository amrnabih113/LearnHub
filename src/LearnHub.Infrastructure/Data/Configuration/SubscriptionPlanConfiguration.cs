using LearnHub.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class SubscriptionPlanConfiguration
    : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("SubscriptionPlans");


        builder.HasKey(x => x.Id);


        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();


        builder.Property(x => x.Tier)
            .HasConversion<string>()
            .HasMaxLength(30);


        builder.Property(x => x.BillingCycle)
            .HasConversion<string>()
            .HasMaxLength(30);


        builder.Property(x => x.IsActive)
            .IsRequired();


        builder.OwnsOne(x => x.Price, money =>
        {
            money.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("PriceAmount");


            money.Property(x => x.Currency)
                .HasMaxLength(3)
                .HasColumnName("PriceCurrency");
        });


        builder.HasIndex(x => new
        {
            x.Tier,
            x.BillingCycle
        })
        .IsUnique();
    }
}