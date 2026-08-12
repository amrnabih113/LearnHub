using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetAdminDashboard;

public sealed record GetAdminDashboardQuery(string? Range = null) : IRequest<Result<AdminDashboardDto>>;
