using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using MediatR;

namespace LearnHub.Application.Features.Courses.Queries.GetCourses;

public sealed record GetCoursesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? CategoryId = null,
    Guid? InstructorId = null,
    CourseLevel? Level = null,
    CourseStatus? Status = null,
    string? Language = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null) : IRequest<Result<PagedResult<CourseDto>>>;