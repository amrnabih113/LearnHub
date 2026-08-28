using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Courses.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.CreateCourse;

public sealed class CreateCourseCommandHandler(
    IAppDbContext context,
    IFileStorageService fileStorageService)
    : IRequestHandler<CreateCourseCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;
    private readonly IFileStorageService _fileStorageService = fileStorageService;

    public async Task<Result<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var instructorExists = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.InstructorId, cancellationToken);
        if (instructorExists == null)
        {
            return ApplicationErrors.UserNotFound;
        }
        if (!instructorExists.Roles.Any(r => r.Role == Domain.Identity.Role.Instructor))
        {
            return Error.Forbidden("ApplicationError.Course.NotInstructor", "User is not an instructor.");
        }
        var categoryExists = await _context.Categories.AnyAsync(x => x.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            return Error.NotFound("ApplicationError.Course.CategoryNotFound", "Category not found.");
        }

        Result<Course> courseResult;
        if (request.Status == CourseStatus.Draft)
        {
            courseResult = Course.CreateDraft(
                id: Guid.NewGuid(),
                title: request.Title,
                instructorId: request.InstructorId,
                categoryId: request.CategoryId,
                description: request.Description,
                price: request.Price,
                level: request.Level,
                language: request.Language,
                languageName: request.LanguageName);
        }
        else
        {
            courseResult = Course.Create(
                id: Guid.NewGuid(),
                title: request.Title,
                description: request.Description,
                instructorId: request.InstructorId,
                categoryId: request.CategoryId,
                thumbnailUrl: null,
                level: request.Level,
                status: request.Status,
                price: request.Price,
                isIncludedInSubscription: request.IsIncludedInSubscription,
                requiredSubscriptionTier: request.RequiredSubscriptionTier,
                language: request.Language,
                languageName: request.LanguageName,
                country: request.Country);
        }

        if (courseResult.IsError)
        {
            return courseResult.Errors;
        }

        var course = courseResult.Value;

        if (request.Thumbnail is not null)
        {
            var uploadResult = await _fileStorageService.UploadImageAsync(
                request.Thumbnail,
                $"courses/{course.Id}/thumbnail",
                cancellationToken);

            if (uploadResult.IsError)
            {
                return uploadResult.Errors;
            }

            var thumbnailResult = course.UpdateThumbnail(uploadResult.Value);
            if (thumbnailResult.IsError)
            {
                return thumbnailResult.Errors;
            }
        }

        await _context.Courses.AddAsync(course, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return course.Id;
    }
}