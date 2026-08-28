using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Search.Dtos;
using LearnHub.Application.Features.Search.Services;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Search.Queries.SearchAutoComplete;

public sealed class SearchAutoCompleteQueryHandler(
    IAppDbContext context,
    ISearchQueryNormalizer normalizer)
    : IRequestHandler<SearchAutoCompleteQuery, Result<SearchAutoCompleteDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ISearchQueryNormalizer _normalizer = normalizer;

    public async Task<Result<SearchAutoCompleteDto>> Handle(
        SearchAutoCompleteQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return new SearchAutoCompleteDto([], [], [], []);
        }

        var normalized = _normalizer.Normalize(request.Query);
        var limit = Math.Clamp(request.MaxResults, 1, 10);

        // Course Suggestions
        var courseSuggestions = await _context.Courses
            .AsNoTracking()
            .Where(c => c.Status == CourseStatus.Published &&
                       (EF.Functions.Like(c.Title, $"%{normalized}%") || EF.Functions.Like(c.Description, $"%{normalized}%")))
            .OrderBy(c => c.Title)
            .Take(limit)
            .Select(c => new AutoCompleteSuggestionDto(c.Id, c.Title, "Course", c.Category != null ? c.Category.Name : null))
            .ToListAsync(cancellationToken);

        // Category Suggestions
        var categorySuggestions = await _context.Categories
            .AsNoTracking()
            .Where(cat => EF.Functions.Like(cat.Name, $"%{normalized}%"))
            .OrderBy(cat => cat.Name)
            .Take(limit)
            .Select(cat => new AutoCompleteSuggestionDto(cat.Id, cat.Name, "Category", null))
            .ToListAsync(cancellationToken);

        // Instructor Suggestions
        var instructorSuggestions = await _context.Users
            .AsNoTracking()
            .Where(u => u.Roles.Any(r => r.Role == Domain.Identity.Role.Instructor) &&
                        (EF.Functions.Like(u.FirstName, $"%{normalized}%") || EF.Functions.Like(u.LastName, $"%{normalized}%")))
            .OrderBy(u => u.FirstName)
            .Take(limit)
            .Select(u => new AutoCompleteSuggestionDto(u.Id, u.FirstName + " " + u.LastName, "Instructor", u.Email))
            .ToListAsync(cancellationToken);

        // Tag Suggestions
        var tagSuggestions = await _context.Tags
            .AsNoTracking()
            .Where(t => EF.Functions.Like(t.Name, $"%{normalized}%"))
            .OrderBy(t => t.Name)
            .Take(limit)
            .Select(t => new AutoCompleteSuggestionDto(t.Id, t.Name, "Tag", null))
            .ToListAsync(cancellationToken);

        return new SearchAutoCompleteDto(
            courseSuggestions,
            categorySuggestions,
            instructorSuggestions,
            tagSuggestions);
    }
}
