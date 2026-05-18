using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Reviews.Enums;
using LearnHub.Domain.Reviews.Events;
using LearnHub.Domain.Reviews.ValueObjects;

namespace LearnHub.Domain.Reviews.InstructorReviews;

public sealed class InstructorReview : AuditableEntity
{
    public string InstructorId { get; private set; } = default!;
    public string StudentId { get; private set; } = default!;
    public Guid? CourseId { get; private set; }
    public Rating Rating { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string Comment { get; private set; } = default!;
    public ReviewStatus Status { get; private set; }

    private InstructorReview() { }

    private InstructorReview(Guid id, string instructorId, string studentId, Guid? courseId, Rating rating, string title, string comment) : base(id)
    {
        InstructorId = instructorId;
        StudentId = studentId;
        CourseId = courseId;
        Rating = rating;
        Title = title;
        Comment = comment;
        Status = ReviewStatus.Draft;
    }

    public static Result<InstructorReview> Create(Guid id, string instructorId, string studentId, Guid? courseId, int rating, string title, string comment)
    {
        if (string.IsNullOrWhiteSpace(instructorId))
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

        var review = new InstructorReview(id, instructorId.Trim(), studentId.Trim(), courseId, ratingResult.Value, title.Trim(), comment.Trim());
        review.AddDomainEvent(new InstructorReviewCreatedDomainEvent(review.Id, review.InstructorId, review.StudentId));

        return review;
    }

    public Result<Updated> Publish()
    {
        if (Status != ReviewStatus.Draft)
        {
            return ReviewErrors.AlreadyPublished;
        }

        Status = ReviewStatus.Published;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        AddDomainEvent(new ReviewPublishedDomainEvent(Id, nameof(InstructorReview)));

        return Result.Updated;
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
}
