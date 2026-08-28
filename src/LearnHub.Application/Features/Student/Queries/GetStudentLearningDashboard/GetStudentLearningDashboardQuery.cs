using LearnHub.Application.Features.Student.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Student.Queries.GetStudentLearningDashboard;

public sealed record GetStudentLearningDashboardQuery(Guid StudentId)
    : IRequest<Result<StudentLearningDashboardDto>>;
