using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.DeleteCourse;

public sealed class DeleteCourseCommandHandler(
    IAppDbContext context,
    IFileStorageService fileStorageService)
    : IRequestHandler<DeleteCourseCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;
    private readonly IFileStorageService _fileStorageService = fileStorageService;

    public async Task<Result<Deleted>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == request.CourseId, cancellationToken);
        if (course is null)
        {
            return Error.NotFound("ApplicationError.Course.NotFound", "Course not found.");
        }

        if (!string.IsNullOrWhiteSpace(course.ThumbnailUrl))
        {
            await _fileStorageService.DeleteImageAsync(course.ThumbnailUrl, cancellationToken);
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}