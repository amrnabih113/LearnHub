using FluentValidation;
using LearnHub.Application.Features.Courses.Options;
using Microsoft.Extensions.Options;

namespace LearnHub.Application.Features.Courses.Commands.CreateCourse;

public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator(IOptions<CourseThumbnailOptions> options)
    {
        var thumbnailOptions = options.Value;

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.InstructorId)
            .NotEmpty();

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Price.Amount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Language)
            .NotEmpty()
            .MaximumLength(5);

        RuleFor(x => x.LanguageName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Thumbnail)
            .Must(file => file is null || file.Length > 0)
            .WithMessage("Thumbnail must not be empty.")
            .Must(file => file is null || file.Length <= thumbnailOptions.MaxImageSizeInBytes)
            .WithMessage("Thumbnail exceeds maximum size.")
            .Must(file => file is null || thumbnailOptions.AllowedImageTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Thumbnail type is not allowed.");
    }
}