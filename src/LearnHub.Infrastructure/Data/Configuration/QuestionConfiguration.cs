using LearnHub.Domain.Assessments.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class QuestionConfiguration
    : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Prompt)
            .IsRequired()
            .HasMaxLength(1000);



        builder.Property(x => x.CorrectTextAnswer)
            .HasMaxLength(1000);



        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(50);



        builder.Property(x => x.Points)
            .IsRequired();



        builder.Property(x => x.Order)
            .IsRequired();



        builder.HasMany(x => x.Choices)
            .WithOne()
            .HasForeignKey("QuestionId")
            .OnDelete(DeleteBehavior.Cascade);



        builder.Ignore(x => x.DomainEvents);
    }
}
