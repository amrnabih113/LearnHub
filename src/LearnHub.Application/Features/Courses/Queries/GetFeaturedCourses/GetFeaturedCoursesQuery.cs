using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Queries.GetFeaturedCourses;

public sealed record GetFeaturedCoursesQuery(int Count = 10) : IRequest<Result<List<CourseDto>>>;