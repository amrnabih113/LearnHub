using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentCategoryId = null) : IRequest<Result<CategoryAdminDto>>;
