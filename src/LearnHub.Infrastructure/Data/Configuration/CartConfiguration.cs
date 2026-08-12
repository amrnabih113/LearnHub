using LearnHub.Domain.Purchasing.Carts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class CartConfiguration
    : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(x => x.Id);


        builder.Property(x => x.StudentId)
            .IsRequired();


        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(x => x.CouponCode)
            .HasMaxLength(50)
            .IsRequired(false);



        builder.HasMany(x => x.Items)
      .WithOne(x => x.Cart)
      .HasForeignKey(x => x.CartId)
      .OnDelete(DeleteBehavior.Cascade);



        builder.HasIndex(x => x.StudentId)
            .IsUnique();



        builder.Ignore(x => x.DomainEvents);
    }
}