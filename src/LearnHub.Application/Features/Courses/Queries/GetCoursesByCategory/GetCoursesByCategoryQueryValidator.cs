using FluentValidation;

namespace LearnHub.Application.Features.Courses.Queries.GetCoursesByCategory;

public sealed class GetCoursesByCategoryQueryValidator : AbstractValidator<GetCoursesByCategoryQuery>
{
    public GetCoursesByCategoryQueryValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}