using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Enrollments.Commands.CancelEnrollment;

public sealed record CancelEnrollmentCommand(Guid EnrollmentId) : IRequest<Result<Updated>>;
