using LearnHub.Application.Features.Instructor.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Instructor.Queries.GetInstructorDashboard;

public sealed record GetInstructorDashboardQuery(Guid InstructorId)
    : IRequest<Result<InstructorDashboardDto>>;
