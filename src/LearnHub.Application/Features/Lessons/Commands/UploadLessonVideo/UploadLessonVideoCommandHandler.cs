using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections.Lessons;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Lessons.Commands.UploadLessonVideo;

public sealed record UploadLessonVideoCommand(
    Guid LessonId,
    Guid InstructorId,
    IFileData VideoFile,
    int DurationInMinutes = 0) : IRequest<Result<string>>;

public sealed class UploadLessonVideoCommandHandler(
    IAppDbContext context,
    IFileStorageService fileStorageService)
    : IRequestHandler<UploadLessonVideoCommand, Result<string>>
{
    private readonly IAppDbContext _context = context;
    private readonly IFileStorageService _fileStorageService = fileStorageService;

    public async Task<Result<string>> Handle(
        UploadLessonVideoCommand request,
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

        var uploadResult = await _fileStorageService.UploadVideoAsync(
            request.VideoFile,
            $"courses/{lesson.Section.CourseId}/lessons/{lesson.Id}/video",
            cancellationToken);

        if (uploadResult.IsError)
        {
            return uploadResult.Errors;
        }

        string videoUrl = uploadResult.Value;
        var updateResult = lesson.UpdateVideo(videoUrl, request.DurationInMinutes > 0 ? request.DurationInMinutes : 1);
        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return videoUrl;
    }
}
