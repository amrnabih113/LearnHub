using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetSubscriptionsAdmin;

public sealed record GetSubscriptionsAdminQuery(
    string? Tier = null,
    string? Status = null,
    Guid? StudentId = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<SubscriptionAdminSummaryDto>>>;
