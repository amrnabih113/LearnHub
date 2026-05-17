using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Courses;

public static class CourseErrors
{
    public static Error TitleRequired
    => Error.Validation(code: "DomainError.Course.TitleRequired",
    description: "Course Title is required");

    public static Error DescriptionRequired
    => Error.Validation(code: "DomainError.Course.DescriptionRequired",
    description: "Course Description is required");

    public static Error InstructorIdRequired
    => Error.Validation(code: "DomainError.Course.InstructorIdRequired",
    description: "Instructor Id is required");

    public static Error InvalidCourseLevel
   => Error.Validation(code: "DomainError.Course.InvalidCourseLevel",
   description: "Course level is invalid. Valid levels are: Beginner, Intermediate, Advanced.");

    public static Error PriceRequired
    => Error.Validation(code: "DomainError.Course.PriceRequired",
    description: "Course Price is Required");

    public static Error InvalidCourseStatus
    => Error.Validation(code: "DomainError.Course.InvalidCourseStatus",
    description: "Course status is invalid. Valid statuses are: Draft, Published, Archived.");

    public static Error SectionsRequired
    => Error.Validation(code: "DomainError.Course.SectionsRequired",
    description: "Course sections are required");

}