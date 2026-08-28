using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections.Lessons;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Lessons.Commands.CreateLesson;

public sealed record CreateLessonCommand(
    Guid SectionId,
    Guid InstructorId,
    string Title,
    string? Description = null,
    string? Content = null,
    bool IsPreview = false,
    int? Order = null) : IRequest<Result<Guid>>;

public sealed class CreateLessonCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateLessonCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Guid>> Handle(
        CreateLessonCommand request,
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

        int nextOrder = request.Order ?? (section.Lessons.Any() ? section.Lessons.Max(l => l.Order) + 1 : 1);

        var lessonResult = Lesson.Create(
            Guid.NewGuid(),
            request.Title,
            request.Description,
            videoUrl: null, // Video can be uploaded later
            isPreview: request.IsPreview,
            content: request.Content,
            durationInMinutes: 0,
            order: nextOrder,
            sectionId: request.SectionId,
            isPublished: false);

        if (lessonResult.IsError)
        {
            return lessonResult.Errors;
        }

        var lesson = lessonResult.Value;
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync(cancellationToken);

        return lesson.Id;
    }
}
