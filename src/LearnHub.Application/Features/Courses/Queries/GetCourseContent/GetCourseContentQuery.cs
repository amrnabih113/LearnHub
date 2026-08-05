using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Queries.GetCourseContent;

public sealed record GetCourseContentQuery(Guid CourseId) : IRequest<Result<CourseContentDto>>;