using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections.Lessons;
using LearnHub.Domain.Courses.Sections.Lessons.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.CreateLesson;

public sealed class CreateLessonCommandHandler(IAppDbContext context) : IRequestHandler<CreateLessonCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Guid>> Handle(CreateLessonCommand request, CancellationToken cancellationToken)
    {
        var section = await _context.Sections.FirstOrDefaultAsync(x => x.Id == request.SectionId, cancellationToken);
        if (section is null)
        {
            return Error.NotFound("ApplicationError.Course.SectionNotFound", "Section not found.");
        }

        var lessonResult = Lesson.Create(
            id: Guid.NewGuid(),
            title: request.Title,
            description: request.Description,
            videoUrl: request.VideoUrl,
            isPreview: request.IsPreview,
            content: request.Content,
            durationInMinutes: request.DurationInMinutes,
            order: request.Order,
            sectionId: request.SectionId);

        if (lessonResult.IsError)
        {
            return lessonResult.Errors;
        }

        var lesson = lessonResult.Value;
        lesson.AddDomainEvent(new LessonCreatedDomainEvent(lesson.Id, section.CourseId));

        await _context.Lessons.AddAsync(lesson, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return lesson.Id;
    }
}