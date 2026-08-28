using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Lessons.Commands.ReorderLessons;

public sealed record LessonOrderItem(Guid LessonId, int Order);

public sealed record ReorderLessonsCommand(
    Guid SectionId,
    Guid InstructorId,
    IReadOnlyList<LessonOrderItem> Items) : IRequest<Result<Updated>>;

public sealed class ReorderLessonsCommandHandler(IAppDbContext context)
    : IRequestHandler<ReorderLessonsCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        ReorderLessonsCommand request,
        CancellationToken cancellationToken)
    {
        var section = await _context.Sections
            .Include(s => s.Course)
            .Include(s => s.Lessons)
            .FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken);

        if (section is null)
        {
            return Error.NotFound("Section.NotFound", "Section was not found.");
        }

        if (section.Course != null && section.Course.InstructorId != request.InstructorId)
        {
            return Error.Forbidden("Course.Forbidden", "Instructor does not own this course.");
        }

        var orderValues = request.Items.Select(i => i.Order).ToList();
        if (orderValues.Distinct().Count() != orderValues.Count)
        {
            return Error.Validation("Lessons.DuplicateOrder", "Duplicate order values are not allowed.");
        }

        foreach (var item in request.Items)
        {
            var lesson = section.Lessons.FirstOrDefault(l => l.Id == item.LessonId);
            if (lesson != null)
            {
                lesson.UpdateOrder(item.Order);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
