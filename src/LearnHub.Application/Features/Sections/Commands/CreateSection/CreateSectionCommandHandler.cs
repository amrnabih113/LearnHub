using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Sections.Commands.CreateSection;

public sealed record CreateSectionCommand(
    Guid CourseId,
    Guid InstructorId,
    string Title,
    string? Description,
    int? Order = null) : IRequest<Result<Guid>>;

public sealed class CreateSectionCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateSectionCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Guid>> Handle(
        CreateSectionCommand request,
        CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(c => c.Sections)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
        {
            return Error.NotFound("Course.NotFound", "Course was not found.");
        }

        if (course.InstructorId != request.InstructorId)
        {
            return Error.Forbidden("Course.Forbidden", "Instructor does not own this course.");
        }

        int nextOrder = request.Order ?? (course.Sections.Any() ? course.Sections.Max(s => s.Order) + 1 : 1);

        var sectionResult = Section.Create(
            Guid.NewGuid(),
            request.Title,
            request.Description ?? string.Empty,
            nextOrder,
            request.CourseId,
            isPublished: false);

        if (sectionResult.IsError)
        {
            return sectionResult.Errors;
        }

        var section = sectionResult.Value;
        _context.Sections.Add(section);
        await _context.SaveChangesAsync(cancellationToken);

        return section.Id;
    }
}
