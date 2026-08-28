using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections.Lessons;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Lessons.Commands.DeleteLesson;

public sealed record DeleteLessonCommand(Guid LessonId, Guid InstructorId)
    : IRequest<Result<Deleted>>;

public sealed class DeleteLessonCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteLessonCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Deleted>> Handle(
        DeleteLessonCommand request,
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

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Deleted;
    }
}
