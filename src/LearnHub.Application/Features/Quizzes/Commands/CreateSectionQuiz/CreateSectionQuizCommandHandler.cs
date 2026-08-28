using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Assessments;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Quizzes.Commands.CreateSectionQuiz;

public sealed record CreateSectionQuizCommand(
    Guid CourseId,
    Guid SectionId,
    string Title,
    string? Description,
    int? TimeLimitMinutes,
    int MaxAttempts,
    int PassPercentage) : IRequest<Result<Guid>>;

public sealed class CreateSectionQuizCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateSectionQuizCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Guid>> Handle(
        CreateSectionQuizCommand request,
        CancellationToken cancellationToken)
    {
        var section = await _context.Sections
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SectionId && s.CourseId == request.CourseId, cancellationToken);

        if (section is null)
        {
            return Error.NotFound("Section.NotFound", "Section was not found in specified course.");
        }

        var quizResult = Quiz.CreateSectionQuiz(
            Guid.NewGuid(),
            request.CourseId,
            request.SectionId,
            request.Title,
            request.Description,
            request.TimeLimitMinutes,
            request.MaxAttempts,
            request.PassPercentage);

        if (quizResult.IsError)
        {
            return quizResult.Errors;
        }

        var quiz = quizResult.Value;
        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync(cancellationToken);

        return quiz.Id;
    }
}
