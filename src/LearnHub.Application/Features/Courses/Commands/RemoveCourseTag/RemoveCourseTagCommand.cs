using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.RemoveCourseTag;

public sealed record RemoveCourseTagCommand(Guid CourseId, Guid TagId) : IRequest<Result<Updated>>;