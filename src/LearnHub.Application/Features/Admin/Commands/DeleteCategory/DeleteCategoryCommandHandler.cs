using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Classification;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteCategoryCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Deleted>> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null)
        {
            return CategoryErrors.CategoryNotFound;
        }

        var hasSubcategories = await _context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.ParentCategoryId == request.Id, cancellationToken);

        if (hasSubcategories)
        {
            return CategoryErrors.CategoryHasSubcategories;
        }

        var hasCourses = await _context.Courses
            .AsNoTracking()
            .AnyAsync(c => c.CategoryId == request.Id, cancellationToken);

        if (hasCourses)
        {
            return CategoryErrors.CategoryHasCourses;
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
