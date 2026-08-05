using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.UpdateCourse;

public sealed class UpdateCourseCommandHandler(
    IAppDbContext context,
    IFileStorageService fileStorageService)
    : IRequestHandler<UpdateCourseCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly IFileStorageService _fileStorageService = fileStorageService;

    public async Task<Result<Updated>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == request.CourseId, cancellationToken);
        if (course is null)
        {
            return Error.NotFound("ApplicationError.Course.NotFound", "Course not found.");
        }

        var categoryExists = await _context.Categories.AnyAsync(x => x.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            return Error.NotFound("ApplicationError.Course.CategoryNotFound", "Category not found.");
        }

        string? thumbnailUrl = course.ThumbnailUrl;

        if (request.Thumbnail is not null)
        {
            if (!string.IsNullOrWhiteSpace(course.ThumbnailUrl))
            {
                var deleteResult = await _fileStorageService.DeleteImageAsync(course.ThumbnailUrl, cancellationToken);
                if (deleteResult.IsError)
                {
                    return deleteResult.Errors;
                }
            }

            var uploadResult = await _fileStorageService.UploadImageAsync(
                request.Thumbnail,
                $"courses/{course.Id}/thumbnail",
                cancellationToken);

            if (uploadResult.IsError)
            {
                return uploadResult.Errors;
            }

            thumbnailUrl = uploadResult.Value;
        }

        var updateResult = course.Update(
            request.Title,
            request.Description,
            request.CategoryId,
            thumbnailUrl,
            request.Level,
            request.Status,
            request.Price,
            request.IsIncludedInSubscription,
            request.RequiredSubscriptionTier,
            request.Language,
            request.LanguageName,
            request.Country);

        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}