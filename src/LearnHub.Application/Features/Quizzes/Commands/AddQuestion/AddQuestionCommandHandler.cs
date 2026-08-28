using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Assessments;
using LearnHub.Domain.Assessments.Enums;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Quizzes.Commands.AddQuestion;

public sealed record ChoiceInput(string Text, bool IsCorrect);

public sealed record AddQuestionCommand(
    Guid QuizId,
    string Prompt,
    QuestionType Type,
    int Points,
    int Order,
    IReadOnlyList<ChoiceInput>? Choices = null,
    string? CorrectTextAnswer = null) : IRequest<Result<Guid>>;

public sealed class AddQuestionCommandHandler(IAppDbContext context)
    : IRequestHandler<AddQuestionCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Guid>> Handle(
        AddQuestionCommand request,
        CancellationToken cancellationToken)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.Choices)
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, cancellationToken);

        if (quiz is null)
        {
            return QuizErrors.QuestionNotFound;
        }

        Guid questionId = Guid.NewGuid();
        var addQuestionResult = quiz.AddQuestion(questionId, request.Prompt, request.Type, request.Points, request.Order);
        if (addQuestionResult.IsError)
        {
            return addQuestionResult.Errors;
        }

        if (request.Type is QuestionType.MultipleChoice or QuestionType.TrueFalse && request.Choices != null)
        {
            foreach (var c in request.Choices)
            {
                var choiceResult = quiz.AddChoice(questionId, Guid.NewGuid(), c.Text, c.IsCorrect);
                if (choiceResult.IsError) return choiceResult.Errors;
            }
        }
        else if (request.Type == QuestionType.ShortAnswer && !string.IsNullOrWhiteSpace(request.CorrectTextAnswer))
        {
            var textResult = quiz.SetCorrectTextAnswer(questionId, request.CorrectTextAnswer);
            if (textResult.IsError) return textResult.Errors;
        }

        var addedQuestion = quiz.Questions.FirstOrDefault(q => q.Id == questionId);
        if (addedQuestion != null)
        {
            _context.Entry(addedQuestion).State = EntityState.Added;
            foreach (var choice in addedQuestion.Choices)
            {
                _context.Entry(choice).State = EntityState.Added;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return questionId;
    }
}
