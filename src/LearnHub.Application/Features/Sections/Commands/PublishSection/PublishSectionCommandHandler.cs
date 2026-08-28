using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Sections.Commands.PublishSection;

public sealed record PublishSectionCommand(Guid SectionId, Guid InstructorId)
    : IRequest<Result<Updated>>;

public sealed class PublishSectionCommandHandler(IAppDbContext context)
    : IRequestHandler<PublishSectionCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        PublishSectionCommand request,
        CancellationToken cancellationToken)
    {
        var section = await _context.Sections
            .Include(s => s.Course)
            .Include(s => s.Lessons)
            .FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken);

        if (section is null)
        {
            return SectionErrors.NotFound;
        }

        if (section.Course != null && section.Course.InstructorId != request.InstructorId)
        {
            return Error.Forbidden("Course.Forbidden", "Instructor does not own this course.");
        }

        section.Publish();
        foreach (var lesson in section.Lessons)
        {
            lesson.Publish();
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
