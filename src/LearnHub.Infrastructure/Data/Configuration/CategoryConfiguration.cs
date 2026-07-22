using LearnHub.Domain.Classification.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class CategoryConfiguration
    : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);



        // Properties

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);


        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(100);


        builder.Property(x => x.Description)
            .HasMaxLength(500);



        // Unique Constraints

        builder.HasIndex(x => x.Name)
            .IsUnique();


        builder.HasIndex(x => x.Slug)
            .IsUnique();



        // Category -> Courses

        builder.HasMany(x => x.Courses)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);



        // Self Referencing Category Hierarchy

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(x => x.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);



        // Domain Events

        builder.Ignore(x => x.DomainEvents);
    }
}