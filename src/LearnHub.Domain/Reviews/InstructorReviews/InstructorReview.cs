using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Reviews.Enums;
using LearnHub.Domain.Reviews.Events;
using LearnHub.Domain.Reviews.ValueObjects;

namespace LearnHub.Domain.Reviews.InstructorReviews;

public sealed class InstructorReview : AuditableEntity
{
    public Guid InstructorId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid? CourseId { get; private set; }
    public Rating Rating { get; private set; } = default!;
    public string Comment { get; private set; } = default!;
    public ReviewStatus Status { get; private set; }

    private InstructorReview() { }

    private InstructorReview(Guid id, Guid instructorId, Guid studentId, Guid? courseId, Rating rating, string comment) : base(id)
    {
        InstructorId = instructorId;
        StudentId = studentId;
        CourseId = courseId;
        Rating = rating;
        Comment = comment;
        Status = ReviewStatus.Draft;
    }

    public static Result<InstructorReview> Create(Guid id, Guid instructorId, Guid studentId, Guid? courseId, int rating, string comment)
    {
        if (instructorId == Guid.Empty)
        {
            return ReviewErrors.TargetIdRequired;
        }
        if (studentId == Guid.Empty)
        {
            return ReviewErrors.StudentIdRequired;
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

        var review = new InstructorReview(id, instructorId, studentId, courseId, ratingResult.Value, comment.Trim());
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

    public Result<Updated> Update(int rating, string comment)
    {
        if (Status != ReviewStatus.Draft)
        {
            return ReviewErrors.NotDraft;
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
