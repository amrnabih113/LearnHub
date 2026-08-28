using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Assessments;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Quizzes.Commands.PublishQuiz;

public sealed record PublishQuizCommand(Guid QuizId) : IRequest<Result<Updated>>;

public sealed class PublishQuizCommandHandler(IAppDbContext context)
    : IRequestHandler<PublishQuizCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        PublishQuizCommand request,
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

        var publishResult = quiz.Publish();
        if (publishResult.IsError)
        {
            return publishResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
