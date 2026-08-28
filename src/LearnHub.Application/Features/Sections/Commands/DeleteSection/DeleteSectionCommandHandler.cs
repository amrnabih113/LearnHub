using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Sections.Commands.DeleteSection;

public sealed record DeleteSectionCommand(Guid SectionId, Guid InstructorId)
    : IRequest<Result<Deleted>>;

public sealed class DeleteSectionCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteSectionCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Deleted>> Handle(
        DeleteSectionCommand request,
        CancellationToken cancellationToken)
    {
        var section = await _context.Sections
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken);

        if (section is null)
        {
            return SectionErrors.NotFound;
        }

        if (section.Course != null && section.Course.InstructorId != request.InstructorId)
        {
            return Error.Forbidden("Course.Forbidden", "Instructor does not own this course.");
        }

        _context.Sections.Remove(section);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Deleted;
    }
}
