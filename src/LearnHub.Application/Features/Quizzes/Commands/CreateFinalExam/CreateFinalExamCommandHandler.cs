using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Assessments;
using LearnHub.Domain.Assessments.Enums;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Quizzes.Commands.CreateFinalExam;

public sealed record CreateFinalExamCommand(
    Guid CourseId,
    string Title,
    string? Description,
    int? TimeLimitMinutes,
    int MaxAttempts,
    int PassPercentage) : IRequest<Result<Guid>>;

public sealed class CreateFinalExamCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateFinalExamCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Guid>> Handle(
        CreateFinalExamCommand request,
        CancellationToken cancellationToken)
    {
        var courseExists = await _context.Courses.AsNoTracking().AnyAsync(c => c.Id == request.CourseId, cancellationToken);
        if (!courseExists)
        {
            return Error.NotFound("Course.NotFound", "Course was not found.");
        }

        var existingFinalExam = await _context.Quizzes
            .AsNoTracking()
            .AnyAsync(q => q.CourseId == request.CourseId && q.Type == QuizType.Final, cancellationToken);

        if (existingFinalExam)
        {
            return QuizErrors.FinalExamAlreadyExists;
        }

        var quizResult = Quiz.CreateFinalExam(
            Guid.NewGuid(),
            request.CourseId,
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
