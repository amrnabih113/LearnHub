using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetTagsAdmin;

public sealed record GetTagsAdminQuery(
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<TagAdminDto>>>;
