using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetUsersAdmin;

public sealed record GetUsersAdminQuery(
    string? Search = null,
    string? Role = null,
    bool? IsEmailVerified = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<UserAdminSummaryDto>>>;
