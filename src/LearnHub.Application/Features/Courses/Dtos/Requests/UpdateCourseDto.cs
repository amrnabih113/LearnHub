using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Subscriptions;

namespace LearnHub.Application.Features.Courses.Dtos.Requests;

public sealed class UpdateCourseDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public CourseLevel Level { get; set; }
    public decimal Price { get; set; }
    public LanguageDto Language { get; set; } = null!;
    public string? Country { get; set; }
    public bool IsIncludedInSubscription { get; set; }
    public SubscriptionTier RequiredSubscriptionTier { get; set; }
}
