using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Queries.GetInstructorCourses;

public sealed record GetInstructorCoursesQuery(Guid InstructorId, int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResult<CourseDto>>>;