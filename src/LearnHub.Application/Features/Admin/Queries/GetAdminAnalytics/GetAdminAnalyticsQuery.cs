using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetAdminAnalytics;

public sealed record GetAdminAnalyticsQuery(int MonthsBack = 6)
    : IRequest<Result<AdminAnalyticsDto>>;
