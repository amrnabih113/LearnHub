using LearnHub.Domain.Common;
using LearnHub.Domain.Courses;

namespace LearnHub.Domain.Classification.Tags;

public sealed class CourseTag
{
    public Guid CourseId { get; private set; }

    public Course Course { get; private set; } = default!;


    public Guid TagId { get; private set; }

    public Tag Tag { get; private set; } = default!;


    private CourseTag()
    {
    }
}