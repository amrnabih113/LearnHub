using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetSubscriptionByIdAdmin;

public sealed record GetSubscriptionByIdAdminQuery(Guid Id) : IRequest<Result<SubscriptionAdminDetailDto>>;
