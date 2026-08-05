using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.UpdateLesson;

public sealed class UpdateLessonCommandHandler(IAppDbContext context) : IRequestHandler<UpdateLessonCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _context.Lessons.FirstOrDefaultAsync(x => x.Id == request.LessonId, cancellationToken);
        if (lesson is null)
        {
            return Error.NotFound("ApplicationError.Course.LessonNotFound", "Lesson not found.");
        }

        var result = lesson.Update(
            request.Title,
            request.Description,
            request.VideoUrl,
            request.IsPreview,
            request.Content,
            request.DurationInMinutes,
            request.Order);

        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}