using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Enrollments.Commands.CompleteEnrollment;

public sealed record CompleteEnrollmentCommand(Guid EnrollmentId) : IRequest<Result<Updated>>;
