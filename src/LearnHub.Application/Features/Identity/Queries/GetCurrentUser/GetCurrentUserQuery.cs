using LearnHub.Application.Features.Identity.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Identity.Queries.GetCurrentUser;


public sealed record GetCurrentUserQuery : IRequest<Result<UserDto>>;