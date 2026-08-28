using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Search.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Subscriptions;
using MediatR;

namespace LearnHub.Application.Features.Search.Queries.SearchCourses;

public sealed record SearchCoursesQuery(
    string? SearchTerm = null,
    Guid? CategoryId = null,
    IReadOnlyList<Guid>? TagIds = null,
    CourseLevel? Level = null,
    string? Language = null,
    bool? IsFree = null,
    bool? IsIncludedInSubscription = null,
    SubscriptionTier? RequiredSubscriptionTier = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    double? MinimumRating = null,
    Guid? InstructorId = null,
    SearchCourseSortBy SortBy = SearchCourseSortBy.Relevance,
    int PageNumber = 1,
    int PageSize = 10,
    Guid? CurrentUserId = null
) : IRequest<Result<PagedResult<CourseSearchDto>>>;
