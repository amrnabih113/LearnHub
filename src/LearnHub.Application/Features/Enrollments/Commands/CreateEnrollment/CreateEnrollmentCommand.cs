using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Enrollments.Commands.CreateEnrollment;

public sealed record CreateEnrollmentCommand(
    Guid StudentId,
    Guid CourseId) : IRequest<Result<Guid>>;
