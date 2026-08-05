using LearnHub.Domain.Courses.Enums;

namespace LearnHub.Application.Features.Courses.Dtos.Requests;

public sealed class CreateCourseDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid InstructorId { get; set; }
    public Guid CategoryId { get; set; }
    public CourseLevel Level { get; set; }
    public decimal Price { get; set; }
    public LanguageDto Language { get; set; } = null!;
    public string? Country { get; set; }
}
