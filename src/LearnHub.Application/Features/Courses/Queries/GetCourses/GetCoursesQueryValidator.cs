using FluentValidation;

namespace LearnHub.Application.Features.Courses.Queries.GetCourses;

public sealed class GetCoursesQueryValidator : AbstractValidator<GetCoursesQuery>
{
    public GetCoursesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(x => x.MinPrice).When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue);
    }
}