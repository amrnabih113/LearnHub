using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Sections.Commands.ReorderSections;

public sealed record SectionOrderItem(Guid SectionId, int Order);

public sealed record ReorderSectionsCommand(
    Guid CourseId,
    Guid InstructorId,
    IReadOnlyList<SectionOrderItem> Items) : IRequest<Result<Updated>>;

public sealed class ReorderSectionsCommandHandler(IAppDbContext context)
    : IRequestHandler<ReorderSectionsCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        ReorderSectionsCommand request,
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

        var orderValues = request.Items.Select(i => i.Order).ToList();
        if (orderValues.Distinct().Count() != orderValues.Count)
        {
            return Error.Validation("Sections.DuplicateOrder", "Duplicate order values are not allowed.");
        }

        foreach (var item in request.Items)
        {
            var section = course.Sections.FirstOrDefault(s => s.Id == item.SectionId);
            if (section != null)
            {
                section.UpdateOrder(item.Order);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
