using LearnHub.Domain.Courses.Enums;

namespace LearnHub.Contracts.Courses.Requests;

public sealed record ChangeCourseStatusRequest(CourseStatus Status);
