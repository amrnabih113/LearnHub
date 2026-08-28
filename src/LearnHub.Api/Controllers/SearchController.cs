using System.Security.Claims;
using LearnHub.Application.Features.Search.Dtos;
using LearnHub.Application.Features.Search.Queries.SearchAutoComplete;
using LearnHub.Application.Features.Search.Queries.SearchCourses;
using LearnHub.Contracts.Certificates.Responses;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Subscriptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/search")]
public sealed class SearchController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpGet("courses")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchCourses(
        [FromQuery] string? q,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid[]? tagIds,
        [FromQuery] CourseLevel? level,
        [FromQuery] string? language,
        [FromQuery] bool? isFree,
        [FromQuery] bool? isIncludedInSubscription,
        [FromQuery] SubscriptionTier? requiredSubscriptionTier,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] double? minimumRating,
        [FromQuery] Guid? instructorId,
        [FromQuery] SearchCourseSortBy sortBy = SearchCourseSortBy.Relevance,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();

        var query = new SearchCoursesQuery(
            SearchTerm: q,
            CategoryId: categoryId,
            TagIds: tagIds,
            Level: level,
            Language: language,
            IsFree: isFree,
            IsIncludedInSubscription: isIncludedInSubscription,
            RequiredSubscriptionTier: requiredSubscriptionTier,
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            MinimumRating: minimumRating,
            InstructorId: instructorId,
            SortBy: sortBy,
            PageNumber: pageNumber,
            PageSize: pageSize,
            CurrentUserId: currentUserId == Guid.Empty ? null : currentUserId);

        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("autocomplete")]
    [AllowAnonymous]
    public async Task<IActionResult> AutoComplete(
        [FromQuery] string q,
        [FromQuery] int limit = 5,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchAutoCompleteQuery(q, limit);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
    }
}
