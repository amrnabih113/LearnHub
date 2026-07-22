using LearnHub.Domain.Purchasing.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class PaymentConfiguration
    : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.OrderId)
            .IsRequired();



        builder.Property(x => x.Provider)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();



        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();



        builder.OwnsOne(x => x.Amount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2);


            money.Property(x => x.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3);
        });



        builder.Property(x => x.TransactionId)
            .HasMaxLength(200);


        builder.Property(x => x.ProviderReference)
            .HasMaxLength(200);


        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);


        builder.Property(x => x.RefundReason)
            .HasMaxLength(500);



        builder.Ignore(x => x.DomainEvents);
    }
}