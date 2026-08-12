using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Commands.CreateTag;

public sealed record CreateTagCommand(
    string Name,
    string Slug,
    string? Description = null) : IRequest<Result<TagAdminDto>>;
