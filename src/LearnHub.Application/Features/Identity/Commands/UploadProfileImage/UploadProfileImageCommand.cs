using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Identity.Commands.UploadProfileImage;

public sealed record UploadProfileImageCommand(
    IFileData Image)
    : IRequest<Result<string>>;