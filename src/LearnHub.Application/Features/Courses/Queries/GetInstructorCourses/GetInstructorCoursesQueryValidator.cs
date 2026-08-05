using FluentValidation;

namespace LearnHub.Application.Features.Courses.Queries.GetInstructorCourses;

public sealed class GetInstructorCoursesQueryValidator : AbstractValidator<GetInstructorCoursesQuery>
{
    public GetInstructorCoursesQueryValidator()
    {
        RuleFor(x => x.InstructorId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}