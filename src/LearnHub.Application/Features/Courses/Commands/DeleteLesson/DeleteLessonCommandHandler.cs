using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.DeleteLesson;

public sealed class DeleteLessonCommandHandler(IAppDbContext context) : IRequestHandler<DeleteLessonCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Deleted>> Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _context.Lessons.FirstOrDefaultAsync(x => x.Id == request.LessonId, cancellationToken);
        if (lesson is null)
        {
            return Error.NotFound("ApplicationError.Course.LessonNotFound", "Lesson not found.");
        }

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}