using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Reviews.Enums;
using LearnHub.Domain.Reviews.Events;
using LearnHub.Domain.Reviews.ValueObjects;

namespace LearnHub.Domain.Reviews.CourseReviews;

public sealed class CourseReview : AuditableEntity
{
    public Guid CourseId { get; private set; }
    public string StudentId { get; private set; } = default!;
    public Rating Rating { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string Comment { get; private set; } = default!;
    public ReviewStatus Status { get; private set; }
    public int HelpfulCount { get; private set; }
    public int NotHelpfulCount { get; private set; }

    private CourseReview() { }

    private CourseReview(Guid id, Guid courseId, string studentId, Rating rating, string title, string comment) : base(id)
    {
        CourseId = courseId;
        StudentId = studentId;
        Rating = rating;
        Title = title;
        Comment = comment;
        Status = ReviewStatus.Draft;
    }

    public static Result<CourseReview> Create(Guid id, Guid courseId, string studentId, int rating, string title, string comment)
    {
        if (courseId == Guid.Empty)
        {
            return ReviewErrors.TargetIdRequired;
        }

        if (string.IsNullOrWhiteSpace(studentId))
        {
            return ReviewErrors.StudentIdRequired;
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return ReviewErrors.TitleRequired;
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            return ReviewErrors.CommentRequired;
        }

        var ratingResult = Rating.Create(rating);
        if (ratingResult.IsError)
        {
            return ratingResult.Errors;
        }

        var review = new CourseReview(id, courseId, studentId.Trim(), ratingResult.Value, title.Trim(), comment.Trim());
        review.AddDomainEvent(new CourseReviewCreatedDomainEvent(review.Id, review.CourseId, review.StudentId));

        return review;
    }

    public Result<Updated> Update(int rating, string title, string comment)
    {
        if (Status != ReviewStatus.Draft)
        {
            return ReviewErrors.NotDraft;
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return ReviewErrors.TitleRequired;
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            return ReviewErrors.CommentRequired;
        }

        var ratingResult = Rating.Create(rating);
        if (ratingResult.IsError)
        {
            return ratingResult.Errors;
        }

        Rating = ratingResult.Value;
        Title = title.Trim();
        Comment = comment.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> Publish()
    {
        if (Status != ReviewStatus.Draft)
        {
            return ReviewErrors.AlreadyPublished;
        }

        Status = ReviewStatus.Published;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        AddDomainEvent(new ReviewPublishedDomainEvent(Id, nameof(CourseReview)));

        return Result.Updated;
    }

    public Result<Updated> Flag()
    {
        if (Status == ReviewStatus.Removed)
        {
            return ReviewErrors.NotPublished;
        }

        Status = ReviewStatus.Flagged;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> Hide()
    {
        if (Status == ReviewStatus.Removed)
        {
            return ReviewErrors.NotPublished;
        }

        Status = ReviewStatus.Hidden;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> Remove()
    {
        Status = ReviewStatus.Removed;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> MarkHelpful()
    {
        if (Status != ReviewStatus.Published)
        {
            return ReviewErrors.NotPublished;
        }

        HelpfulCount++;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> MarkNotHelpful()
    {
        if (Status != ReviewStatus.Published)
        {
            return ReviewErrors.NotPublished;
        }

        NotHelpfulCount++;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }
}
