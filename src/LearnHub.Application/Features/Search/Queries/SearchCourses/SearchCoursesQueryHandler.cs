using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Search.Dtos;
using LearnHub.Application.Features.Search.Services;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Enrollments.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Search.Queries.SearchCourses;

public sealed class SearchCoursesQueryHandler(
    IAppDbContext context,
    ISearchQueryNormalizer normalizer,
    ICourseAccessService accessService)
    : IRequestHandler<SearchCoursesQuery, Result<PagedResult<CourseSearchDto>>>
{
    private readonly IAppDbContext _context = context;
    private readonly ISearchQueryNormalizer _normalizer = normalizer;
    private readonly ICourseAccessService _accessService = accessService;

    public async Task<Result<PagedResult<CourseSearchDto>>> Handle(
        SearchCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var baseQuery = _context.Courses
            .AsNoTracking()
            .Where(c => c.Status == CourseStatus.Published);

        // Filter: Category
        if (request.CategoryId.HasValue)
        {
            baseQuery = baseQuery.Where(c => c.CategoryId == request.CategoryId.Value);
        }

        // Filter: Instructor
        if (request.InstructorId.HasValue)
        {
            baseQuery = baseQuery.Where(c => c.InstructorId == request.InstructorId.Value);
        }

        // Filter: Level
        if (request.Level.HasValue)
        {
            baseQuery = baseQuery.Where(c => c.Level == request.Level.Value);
        }

        // Filter: Language
        if (!string.IsNullOrWhiteSpace(request.Language))
        {
            var lang = request.Language.Trim();
            baseQuery = baseQuery.Where(c => c.Language.Code == lang || c.Language.Name == lang);
        }

        // Filter: Free / Paid
        if (request.IsFree.HasValue)
        {
            if (request.IsFree.Value)
            {
                baseQuery = baseQuery.Where(c => c.Price.Amount == 0);
            }
            else
            {
                baseQuery = baseQuery.Where(c => c.Price.Amount > 0);
            }
        }

        // Filter: Price Range
        if (request.MinPrice.HasValue)
        {
            baseQuery = baseQuery.Where(c => c.Price.Amount >= request.MinPrice.Value);
        }
        if (request.MaxPrice.HasValue)
        {
            baseQuery = baseQuery.Where(c => c.Price.Amount <= request.MaxPrice.Value);
        }

        // Filter: Subscription Tier
        if (request.IsIncludedInSubscription.HasValue)
        {
            baseQuery = baseQuery.Where(c => c.IsIncludedInSubscription == request.IsIncludedInSubscription.Value);
        }
        if (request.RequiredSubscriptionTier.HasValue)
        {
            baseQuery = baseQuery.Where(c => c.RequiredSubscriptionTier == request.RequiredSubscriptionTier.Value);
        }

        // Filter: Tags
        if (request.TagIds is not null && request.TagIds.Count > 0)
        {
            baseQuery = baseQuery.Where(c => c.CourseTags.Any(ct => request.TagIds.Contains(ct.TagId)));
        }

        // Filter: Minimum Rating
        if (request.MinimumRating.HasValue && request.MinimumRating.Value > 0)
        {
            var minRating = request.MinimumRating.Value;
            baseQuery = baseQuery.Where(c => _context.CourseReviews
                .Where(r => r.CourseId == c.Id)
                .Select(r => (double?)r.Rating.Value)
                .Average() >= minRating);
        }

        // Search Term & Relevance Scoring across 6 Metadata Layers
        var searchTerm = _normalizer.Normalize(request.SearchTerm ?? string.Empty);
        var hasSearchTerm = !string.IsNullOrEmpty(searchTerm);

        if (hasSearchTerm)
        {
            baseQuery = baseQuery.Where(c =>
                EF.Functions.Like(c.Title, $"%{searchTerm}%") ||
                EF.Functions.Like(c.Description, $"%{searchTerm}%") ||
                (c.Category != null && (EF.Functions.Like(c.Category.Name, $"%{searchTerm}%") || (c.Category.Description != null && EF.Functions.Like(c.Category.Description, $"%{searchTerm}%")))) ||
                c.CourseTags.Any(ct => ct.Tag != null && (EF.Functions.Like(ct.Tag.Name, $"%{searchTerm}%") || (ct.Tag.Description != null && EF.Functions.Like(ct.Tag.Description, $"%{searchTerm}%")))) ||
                (c.Instructor != null && (EF.Functions.Like(c.Instructor.FirstName, $"%{searchTerm}%") || EF.Functions.Like(c.Instructor.LastName, $"%{searchTerm}%") || (c.Instructor.Bio != null && EF.Functions.Like(c.Instructor.Bio, $"%{searchTerm}%")))) ||
                c.Sections.Any(s => EF.Functions.Like(s.Title, $"%{searchTerm}%") || EF.Functions.Like(s.Description, $"%{searchTerm}%") ||
                    s.Lessons.Any(l => (l.Title != null && EF.Functions.Like(l.Title, $"%{searchTerm}%")) ||
                                       (l.Description != null && EF.Functions.Like(l.Description, $"%{searchTerm}%")) ||
                                       (l.Content != null && EF.Functions.Like(l.Content, $"%{searchTerm}%")))));
        }

        // Projection expression with calculated rating & count
        var projectedQuery = baseQuery.Select(c => new
        {
            Course = c,
            CategoryName = c.Category != null ? c.Category.Name : string.Empty,
            InstructorName = c.Instructor != null ? (c.Instructor.FirstName + " " + c.Instructor.LastName) : "LearnHub Instructor",
            AverageRating = _context.CourseReviews
                .Where(r => r.CourseId == c.Id)
                .Select(r => (double?)r.Rating.Value)
                .Average() ?? 0.0,
            RatingCount = _context.CourseReviews.Count(r => r.CourseId == c.Id),
            EnrollmentCount = _context.Enrollments.Count(e => e.CourseId == c.Id),
            RelevanceScore = hasSearchTerm
                ? (c.Title == searchTerm ? 100.0
                    : EF.Functions.Like(c.Title, $"{searchTerm}%") ? 80.0
                    : EF.Functions.Like(c.Title, $"%{searchTerm}%") ? 60.0
                    : (c.Category != null && EF.Functions.Like(c.Category.Name, $"%{searchTerm}%")) ? 40.0
                    : c.CourseTags.Any(ct => ct.Tag != null && EF.Functions.Like(ct.Tag.Name, $"%{searchTerm}%")) ? 40.0
                    : (c.Instructor != null && (EF.Functions.Like(c.Instructor.FirstName, $"%{searchTerm}%") || EF.Functions.Like(c.Instructor.LastName, $"%{searchTerm}%"))) ? 25.0
                    : c.Sections.Any(s => EF.Functions.Like(s.Title, $"%{searchTerm}%")) ? 20.0
                    : 10.0)
                : 0.0
        });

        // Sorting
        var orderedQuery = request.SortBy switch
        {
            SearchCourseSortBy.Newest => projectedQuery.OrderByDescending(x => x.Course.CreatedAtUtc),
            SearchCourseSortBy.Oldest => projectedQuery.OrderBy(x => x.Course.CreatedAtUtc),
            SearchCourseSortBy.PriceLowToHigh => projectedQuery.OrderBy(x => x.Course.Price.Amount),
            SearchCourseSortBy.PriceHighToLow => projectedQuery.OrderByDescending(x => x.Course.Price.Amount),
            SearchCourseSortBy.HighestRated => projectedQuery.OrderByDescending(x => x.AverageRating),
            SearchCourseSortBy.MostPopular => projectedQuery.OrderByDescending(x => x.EnrollmentCount),
            _ => hasSearchTerm
                ? projectedQuery.OrderByDescending(x => x.RelevanceScore).ThenByDescending(x => x.EnrollmentCount)
                : projectedQuery.OrderByDescending(x => x.Course.CreatedAtUtc)
        };

        // Server-side Pagination
        var totalCount = await orderedQuery.CountAsync(cancellationToken);

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        var rawItems = await orderedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Map to CourseSearchDto
        var items = new List<CourseSearchDto>();

        foreach (var item in rawItems)
        {
            var c = item.Course;
            bool isEnrolled = false;
            EnrollmentStatus? enrollmentStatus = null;
            bool canAccess = false;

            if (request.CurrentUserId.HasValue && request.CurrentUserId.Value != Guid.Empty)
            {
                var accessResult = await _accessService.EvaluateAccessAsync(
                    request.CurrentUserId.Value, c.Id, cancellationToken);

                if (accessResult.IsSuccess)
                {
                    canAccess = accessResult.Value.CanWatchLessons;
                    enrollmentStatus = accessResult.Value.Status;
                    isEnrolled = enrollmentStatus.HasValue;
                }
            }

            items.Add(new CourseSearchDto(
                CourseId: c.Id,
                Title: c.Title,
                ThumbnailUrl: c.ThumbnailUrl,
                Description: c.Description,
                CategoryId: c.CategoryId,
                CategoryName: item.CategoryName,
                InstructorId: c.InstructorId,
                InstructorName: item.InstructorName,
                Level: c.Level,
                LanguageCode: c.Language.Code,
                LanguageName: c.Language.Name,
                Price: c.Price.Amount,
                Currency: c.Price.Currency,
                IsFree: c.Price.Amount == 0,
                IsIncludedInSubscription: c.IsIncludedInSubscription,
                RequiredSubscriptionTier: c.RequiredSubscriptionTier,
                AverageRating: Math.Round(item.AverageRating, 1),
                RatingCount: item.RatingCount,
                EnrollmentCount: item.EnrollmentCount,
                CreatedAtUtc: c.CreatedAtUtc,
                RelevanceScore: item.RelevanceScore,
                IsEnrolled: isEnrolled,
                EnrollmentStatus: enrollmentStatus,
                CanAccess: canAccess));
        }

        return new PagedResult<CourseSearchDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}
