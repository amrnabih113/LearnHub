using LearnHub.Domain.Purchasing.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class OrderItemConfiguration
    : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.CourseId)
            .IsRequired();



        builder.Property(x => x.CourseTitle)
            .IsRequired()
            .HasMaxLength(200);



        builder.OwnsOne(
           x => x.UnitPriceSnapshot,
           money =>
           {
               money.Property(x => x.Amount)
                   .HasColumnName("UnitPriceAmount")
                   .HasPrecision(18, 2);


               money.Property(x => x.Currency)
                   .HasColumnName("UnitPriceCurrency")
                   .HasMaxLength(3)
                   .IsRequired();
           });


        builder.Ignore(x => x.DomainEvents);
    }
}