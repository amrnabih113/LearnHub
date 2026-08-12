using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentCategoryId = null) : IRequest<Result<CategoryAdminDto>>;
