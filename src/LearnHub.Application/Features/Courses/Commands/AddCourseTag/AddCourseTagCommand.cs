using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.AddCourseTag;

public sealed record AddCourseTagCommand(Guid CourseId, Guid TagId) : IRequest<Result<Updated>>;