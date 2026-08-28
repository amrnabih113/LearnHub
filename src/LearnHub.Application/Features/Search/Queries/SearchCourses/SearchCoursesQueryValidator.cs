using FluentValidation;

namespace LearnHub.Application.Features.Search.Queries.SearchCourses;

public sealed class SearchCoursesQueryValidator : AbstractValidator<SearchCoursesQuery>
{
    public SearchCoursesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue)
            .WithMessage("Minimum price cannot be negative.");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue)
            .WithMessage("Maximum price cannot be negative.");

        RuleFor(x => x)
            .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice.Value <= x.MaxPrice.Value)
            .WithMessage("MinPrice must be less than or equal to MaxPrice.");

        RuleFor(x => x.MinimumRating)
            .InclusiveBetween(0.0, 5.0)
            .When(x => x.MinimumRating.HasValue)
            .WithMessage("Minimum rating must be between 0 and 5.");
    }
}
