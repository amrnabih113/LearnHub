using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.ChangeCourseStatus;

public sealed record ChangeCourseStatusCommand(Guid CourseId, CourseStatus Status) : IRequest<Result<Updated>>;