using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections.Lessons;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Lessons.Commands.PublishLesson;

public sealed record PublishLessonCommand(Guid LessonId, Guid InstructorId)
    : IRequest<Result<Updated>>;

public sealed class PublishLessonCommandHandler(IAppDbContext context)
    : IRequestHandler<PublishLessonCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        PublishLessonCommand request,
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

        lesson.Publish();
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
