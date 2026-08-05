using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.DeleteCourse;

public sealed record DeleteCourseCommand(Guid CourseId) : IRequest<Result<Deleted>>;