using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.CreateSection;

public sealed class CreateSectionCommandHandler(IAppDbContext context) : IRequestHandler<CreateSectionCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Guid>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        var courseExists = await _context.Courses.AnyAsync(x => x.Id == request.CourseId, cancellationToken);
        if (!courseExists)
        {
            return Error.NotFound("ApplicationError.Course.NotFound", "Course not found.");
        }

        var sectionResult = Section.Create(
            id: Guid.NewGuid(),
            title: request.Title,
            description: request.Description,
            order: request.Order,
            courseId: request.CourseId);

        if (sectionResult.IsError)
        {
            return sectionResult.Errors;
        }

        await _context.Sections.AddAsync(sectionResult.Value, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return sectionResult.Value.Id;
    }
}