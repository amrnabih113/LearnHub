using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Assessments;
using LearnHub.Domain.Assessments.ValueObjects;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Quizzes.Commands.SaveQuizAnswer;

public sealed class SaveQuizAnswerCommandHandler(IAppDbContext context)
    : IRequestHandler<SaveQuizAnswerCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        SaveQuizAnswerCommand request,
        CancellationToken cancellationToken)
    {
        var attempt = await _context.QuizAttempts
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == request.AttemptId, cancellationToken);

        if (attempt is null)
        {
            return QuizAttemptErrors.AttemptNotFound;
        }

        if (attempt.StudentId != request.StudentId)
        {
            return QuizAttemptErrors.StudentMismatch;
        }

        AnswerOption option;
        if (request.SelectedChoiceId.HasValue)
        {
            var optResult = AnswerOption.FromChoice(request.SelectedChoiceId.Value);
            if (optResult.IsError) return optResult.Errors;
            option = optResult.Value;
        }
        else if (!string.IsNullOrWhiteSpace(request.TextAnswer))
        {
            var optResult = AnswerOption.FromText(request.TextAnswer);
            if (optResult.IsError) return optResult.Errors;
            option = optResult.Value;
        }
        else
        {
            return QuizAttemptErrors.OptionRequired;
        }

        var answerResult = attempt.AnswerQuestion(request.QuestionId, option, DateTimeOffset.UtcNow);
        if (answerResult.IsError)
        {
            return answerResult.Errors;
        }

        var lastAnswer = attempt.Answers.FirstOrDefault(a => a.QuestionId == request.QuestionId);
        if (lastAnswer != null && _context.Entry(lastAnswer).State == EntityState.Detached)
        {
            _context.Entry(lastAnswer).State = EntityState.Added;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
