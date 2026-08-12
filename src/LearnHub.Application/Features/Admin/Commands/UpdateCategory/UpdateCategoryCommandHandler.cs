using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Classification;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateCategoryCommand, Result<CategoryAdminDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CategoryAdminDto>> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null)
        {
            return CategoryErrors.CategoryNotFound;
        }

        var duplicateExists = await _context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Id != request.Id && (c.Name == request.Name.Trim() || c.Slug == request.Slug.Trim().ToLowerInvariant()), cancellationToken);

        if (duplicateExists)
        {
            return CategoryErrors.DuplicateName;
        }

        string? parentName = null;
        if (request.ParentCategoryId.HasValue)
        {
            if (request.ParentCategoryId.Value == request.Id)
            {
                return CategoryErrors.HierarchyInvalid;
            }

            var parent = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId.Value, cancellationToken);

            if (parent is null)
            {
                return CategoryErrors.ParentCategoryRequired;
            }

            // Check circular dependency: walk up the parent chain
            var currentParentId = parent.ParentCategoryId;
            while (currentParentId.HasValue)
            {
                if (currentParentId.Value == request.Id)
                {
                    return CategoryErrors.HierarchyInvalid;
                }
                var nextParent = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == currentParentId.Value, cancellationToken);
                currentParentId = nextParent?.ParentCategoryId;
            }

            parentName = parent.Name;
        }

        var renameResult = category.Rename(request.Name, request.Slug, request.Description);
        if (renameResult.IsError)
        {
            return renameResult.Errors;
        }

        var parentResult = category.ChangeParent(request.ParentCategoryId);
        if (parentResult.IsError)
        {
            return parentResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new CategoryAdminDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.ParentCategoryId,
            parentName,
            category.CreatedAtUtc);
    }
}
