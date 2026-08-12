using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetPaymentsAdmin;

public sealed record GetPaymentsAdminQuery(
    string? Status = null,
    string? Provider = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<PaymentAdminSummaryDto>>>;
