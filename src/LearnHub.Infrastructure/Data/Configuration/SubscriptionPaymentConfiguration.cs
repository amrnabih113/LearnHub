using LearnHub.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class SubscriptionPaymentConfiguration
    : IEntityTypeConfiguration<SubscriptionPayment>
{
    public void Configure(EntityTypeBuilder<SubscriptionPayment> builder)
    {
        builder.ToTable("SubscriptionPayments");


        builder.HasKey(x => x.Id);


        builder.Property(x => x.SubscriptionId)
            .IsRequired();

        builder.HasIndex(x => x.SubscriptionId);


        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);


        builder.Property(x => x.AttemptCount)
            .IsRequired();


        builder.Property(x => x.GatewayTransactionId)
            .HasMaxLength(200);

        builder.HasIndex(x => x.GatewayTransactionId);


        builder.Property(x => x.FailureReason)
            .HasMaxLength(1000);


        builder.Property(x => x.RefundReason)
            .HasMaxLength(1000);


        builder.OwnsOne(x => x.Amount, money =>
        {
            money.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("Amount");


            money.Property(x => x.Currency)
                .HasMaxLength(3)
                .HasColumnName("Currency");
        });


        builder.HasOne(x => x.Subscription)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}