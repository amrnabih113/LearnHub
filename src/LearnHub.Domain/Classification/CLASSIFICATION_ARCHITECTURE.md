# Course Classification and Discovery Design (DDD + Clean Architecture)

## Bounded Context

This context keeps category and tag management separate from the `Course` aggregate, but simple.

It includes two aggregates:

- `Category`
- `Tag`

The `Course` aggregate stores only `CategoryId` and `TagIds`.

## Simple Meaning

Categories are for browsing and hierarchy.

Tags are for filtering and search.

Topics can be treated as tags until the product needs a dedicated topic model.

## Why the Design Stays Small

- Categories are separate because they are shared and hierarchical.
- Tags are separate because they are reusable across many courses.
- `Course` uses IDs only so the aggregate stays small.
- The application layer can still enforce uniqueness and active-course checks.

## Category Aggregate

Behaviors:

- `Create(...)`
- `Rename(...)`
- `ChangeParent(...)`
- `Archive()`

Rules:

- Name is required.
- Slug is required and normalized.
- Parent cannot be self.
- Archived categories are not editable.

## Tag Aggregate

Behaviors:

- `Create(...)`
- `Rename(...)`
- `Archive()`

Rules:

- Name is required.
- Slug is required and normalized.
- Archived tags are not editable.

## Course Relationship

- `Course.CategoryId` points to one category.
- `Course.TagIds` holds many tag ids.
- No category or tag object lives inside `Course`.

## Business Rules

- Category names must be unique.
- Category slugs must be unique.
- Parent category cannot reference itself.
- Cannot delete category with active courses.
- Tags cannot be duplicated on the same course.
- Maximum tag count per course.
- Slugs must be URL-friendly.

Uniqueness and active-course checks belong in application/persistence because they need repository lookups.

## Folder Structure

```text
src/LearnHub.Domain/Classification/
  CLASSIFICATION_ARCHITECTURE.md
  ClassificationErrors.cs
  Enums/
    CategoryStatus.cs
    TagStatus.cs
  Errors/
    CategoryErrors.cs
    TagErrors.cs
  Categories/
    Category.cs
  Tags/
    Tag.cs
```

## Clean Architecture Fit

- Domain keeps the rules.
- Application handles validation against repositories.
- Infrastructure stores categories, tags, and course tag links.

This is enough for ASP.NET Core, EF Core, CQRS, MediatR, modular monolith, and later service split.
