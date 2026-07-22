using LearnHub.Domain.Purchasing.Coupons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class CouponConfiguration
    : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);



        builder.HasIndex(x => x.Code)
            .IsUnique();



        builder.Property(x => x.DiscountType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();



        builder.Property(x => x.DiscountValue)
            .HasPrecision(18, 2)
            .IsRequired();



        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);



        builder.Property(x => x.RedemptionCount)
            .IsRequired();



        builder.Property(x => x.IsActive)
            .IsRequired();



        builder.Property(x => x.MaxRedemptions);



        // Allowed courses
        builder.Property(x => x.AllowedCourseIds)
            .HasConversion(
                ids => string.Join(',', ids),
                value => value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList())
            .HasColumnName("AllowedCourseIds");



        builder.Ignore(x => x.DomainEvents);
    }
}