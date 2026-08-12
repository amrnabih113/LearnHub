using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetOrdersAdmin;

public sealed record GetOrdersAdminQuery(
    string? Search = null,
    string? Status = null,
    DateTimeOffset? FromDate = null,
    DateTimeOffset? ToDate = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<OrderAdminSummaryDto>>>;
