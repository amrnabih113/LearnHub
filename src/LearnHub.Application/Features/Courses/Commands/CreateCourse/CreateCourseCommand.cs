using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Subscriptions;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.CreateCourse;

public sealed record CreateCourseCommand(
    string Title,
    string Description,
    Guid InstructorId,
    Guid CategoryId,
    IFileData? Thumbnail,
    CourseLevel Level,
    CourseStatus Status,
    Money Price,
    bool IsIncludedInSubscription,
    SubscriptionTier RequiredSubscriptionTier,
    string Language,
    string LanguageName,
    string? Country) : IRequest<Result<Guid>>;