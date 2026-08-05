using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Queries.GetCourseById;

public sealed record GetCourseByIdQuery(Guid CourseId) : IRequest<Result<CourseDetailsDto>>;