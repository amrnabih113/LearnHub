using FluentValidation;

namespace LearnHub.Application.Features.Courses.Queries.GetCourseContent;

public sealed class GetCourseContentQueryValidator : AbstractValidator<GetCourseContentQuery>
{
    public GetCourseContentQueryValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
    }
}