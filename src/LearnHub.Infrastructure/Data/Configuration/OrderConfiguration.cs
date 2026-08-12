using LearnHub.Domain.Purchasing.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class OrderConfiguration
    : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.StudentId)
            .IsRequired();

        builder.HasIndex(x => x.StudentId);



        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);



        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();



        // Money objects

        builder.OwnsOne(x => x.SubtotalAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("SubtotalAmount")
                .HasPrecision(18, 2);

            money.Property(x => x.Currency)
                .HasColumnName("SubtotalCurrency")
                .HasMaxLength(3);
        });


        builder.OwnsOne(x => x.DiscountAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("DiscountAmount")
                .HasPrecision(18, 2);

            money.Property(x => x.Currency)
                .HasColumnName("DiscountCurrency")
                .HasMaxLength(3);
        });



        builder.OwnsOne(x => x.TotalAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("TotalAmount")
                .HasPrecision(18, 2);

            money.Property(x => x.Currency)
                .HasColumnName("TotalCurrency")
                .HasMaxLength(3);
        });



        // Coupon snapshot

        builder.OwnsOne(x => x.AppliedCoupon, coupon =>
        {
            coupon.Property(x => x.Code)
                .HasColumnName("CouponCode")
                .HasMaxLength(50);

            coupon.Property(x => x.DiscountValue)
                .HasColumnName("CouponDiscountValue")
                .HasPrecision(18, 2);

            coupon.Property(x => x.Currency)
                .HasColumnName("CouponCurrency")
                .HasMaxLength(3);

            coupon.Property(x => x.DiscountType)
                .HasColumnName("CouponDiscountType")
                .HasConversion<string>();
        });


        builder.HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);



        builder.HasMany(x => x.Payments)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);



        builder.Ignore(x => x.DomainEvents);
    }
}