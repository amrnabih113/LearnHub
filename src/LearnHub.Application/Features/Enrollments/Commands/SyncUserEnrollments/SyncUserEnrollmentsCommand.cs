using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Enrollments.Commands.SyncUserEnrollments;

public sealed record SyncUserEnrollmentsCommand(Guid StudentId) : IRequest<Result<Updated>>;
