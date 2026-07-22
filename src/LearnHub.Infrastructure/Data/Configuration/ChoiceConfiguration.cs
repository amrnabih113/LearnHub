using LearnHub.Domain.Assessments.Questions.Choices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class ChoiceConfiguration
    : IEntityTypeConfiguration<Choice>
{
    public void Configure(EntityTypeBuilder<Choice> builder)
    {
        builder.ToTable("Choices");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(500);



        builder.Property(x => x.IsCorrect)
            .IsRequired();



        builder.Ignore(x => x.DomainEvents);
    }
}
