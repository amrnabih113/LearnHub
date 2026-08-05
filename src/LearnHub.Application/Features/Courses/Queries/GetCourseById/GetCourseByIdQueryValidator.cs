using FluentValidation;

namespace LearnHub.Application.Features.Courses.Queries.GetCourseById;

public sealed class GetCourseByIdQueryValidator : AbstractValidator<GetCourseByIdQuery>
{
    public GetCourseByIdQueryValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
    }
}