using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Commands.UpdateTag;

public sealed record UpdateTagCommand(
    Guid Id,
    string Name,
    string Slug,
    string? Description = null) : IRequest<Result<TagAdminDto>>;
