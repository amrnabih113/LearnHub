using LearnHub.Domain.Assessments.Attempts;
using LearnHub.Domain.Assessments.Questions;
using LearnHub.Domain.Assessments.Questions.Choices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class AnswerConfiguration
    : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answers");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.TextAnswer)
            .HasMaxLength(2000);



        builder.Property(x => x.AnsweredAtUtc)
            .IsRequired();



        builder.HasOne<Question>()
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);



        builder.HasOne<Choice>()
            .WithMany()
            .HasForeignKey(x => x.SelectedChoiceId)
            .OnDelete(DeleteBehavior.Restrict);



        builder.Ignore(x => x.DomainEvents);
    }
}