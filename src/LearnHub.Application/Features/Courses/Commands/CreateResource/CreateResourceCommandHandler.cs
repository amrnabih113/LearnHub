using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections.Lessons.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.CreateResource;

public sealed class CreateResourceCommandHandler(IAppDbContext context) : IRequestHandler<CreateResourceCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Guid>> Handle(CreateResourceCommand request, CancellationToken cancellationToken)
    {
        var lessonExists = await _context.Lessons.AnyAsync(x => x.Id == request.LessonId, cancellationToken);
        if (!lessonExists)
        {
            return Error.NotFound("ApplicationError.Course.LessonNotFound", "Lesson not found.");
        }

        var resourceResult = Resource.Create(
            id: Guid.NewGuid(),
            name: request.Name,
            url: request.Url,
            type: request.Type,
            sizeInBytes: request.SizeInBytes,
            lessonId: request.LessonId);

        if (resourceResult.IsError)
        {
            return resourceResult.Errors;
        }

        await _context.Resources.AddAsync(resourceResult.Value, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return resourceResult.Value.Id;
    }
}