using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Classification;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateCategoryCommand, Result<CategoryAdminDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CategoryAdminDto>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var nameOrSlugExists = await _context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Name == request.Name.Trim() || c.Slug == request.Slug.Trim().ToLowerInvariant(), cancellationToken);

        if (nameOrSlugExists)
        {
            return CategoryErrors.DuplicateName;
        }

        string? parentName = null;
        if (request.ParentCategoryId.HasValue)
        {
            var parent = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId.Value, cancellationToken);

            if (parent is null)
            {
                return CategoryErrors.ParentCategoryRequired;
            }

            parentName = parent.Name;
        }

        var categoryResult = Category.Create(Guid.NewGuid(), request.Name, request.Slug, request.Description, request.ParentCategoryId);
        if (categoryResult.IsError)
        {
            return categoryResult.Errors;
        }

        var category = categoryResult.Value;
        _context.Categories.Add(category);
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
