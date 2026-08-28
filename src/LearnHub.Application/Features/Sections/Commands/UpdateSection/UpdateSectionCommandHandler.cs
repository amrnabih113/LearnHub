using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Sections.Commands.UpdateSection;

public sealed record UpdateSectionCommand(
    Guid SectionId,
    Guid InstructorId,
    string Title,
    string? Description) : IRequest<Result<Updated>>;

public sealed class UpdateSectionCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateSectionCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        UpdateSectionCommand request,
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

        var updateResult = section.Update(request.Title, request.Description ?? string.Empty, section.Order);
        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
