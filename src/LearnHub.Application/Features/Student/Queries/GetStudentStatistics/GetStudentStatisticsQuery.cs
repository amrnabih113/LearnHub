using LearnHub.Application.Features.Student.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Student.Queries.GetStudentStatistics;

public sealed record GetStudentStatisticsQuery(Guid StudentId)
    : IRequest<Result<StudentStatisticsDto>>;
