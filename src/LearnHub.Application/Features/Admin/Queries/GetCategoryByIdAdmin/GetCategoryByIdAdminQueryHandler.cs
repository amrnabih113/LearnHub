using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Classification;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetCategoryByIdAdmin;

public sealed class GetCategoryByIdAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCategoryByIdAdminQuery, Result<CategoryAdminDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CategoryAdminDto>> Handle(
        GetCategoryByIdAdminQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null)
        {
            return CategoryErrors.CategoryNotFound;
        }

        string? parentName = null;
        if (category.ParentCategoryId.HasValue)
        {
            var parent = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == category.ParentCategoryId.Value, cancellationToken);
            parentName = parent?.Name;
        }

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
