using LearnHub.Domain.Purchasing.Carts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class CartItemConfiguration
    : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.CourseId)
            .IsRequired();


        builder.Property(x => x.CourseTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(x => x.Cart)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(
       x => x.UnitPrice,
       money =>
       {
           money.Property(x => x.Amount)
               .HasColumnName("UnitPriceAmount")
               .HasPrecision(18, 2);


           money.Property(x => x.Currency)
               .HasColumnName("UnitPriceCurrency")
               .HasMaxLength(3);
       });



        builder.HasIndex(x => new { x.CartId, x.CourseId })
            .IsUnique();

        builder.Ignore(x => x.DomainEvents);
    }
}