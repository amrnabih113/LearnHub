using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections.Lessons;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Lessons.Commands.UpdateLesson;

public sealed record UpdateLessonCommand(
    Guid LessonId,
    Guid InstructorId,
    string Title,
    string? Description = null,
    string? Content = null,
    bool IsPreview = false) : IRequest<Result<Updated>>;

public sealed class UpdateLessonCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateLessonCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        UpdateLessonCommand request,
        CancellationToken cancellationToken)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Section)
                .ThenInclude(s => s.Course)
            .FirstOrDefaultAsync(l => l.Id == request.LessonId, cancellationToken);

        if (lesson is null)
        {
            return LessonErrors.NotFound;
        }

        if (lesson.Section?.Course != null && lesson.Section.Course.InstructorId != request.InstructorId)
        {
            return Error.Forbidden("Course.Forbidden", "Instructor does not own this course.");
        }

        var updateResult = lesson.Update(
            request.Title,
            request.Description,
            lesson.VideoUrl,
            request.IsPreview,
            request.Content,
            lesson.DurationInMinutes,
            lesson.Order);

        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
