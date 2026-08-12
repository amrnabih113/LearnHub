using LearnHub.Application.Features.Enrollments.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Enrollments.Queries.GetEnrollmentById;

public sealed record GetEnrollmentByIdQuery(Guid Id) : IRequest<Result<EnrollmentDetailsDto>>;
