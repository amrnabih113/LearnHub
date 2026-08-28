using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses;

namespace LearnHub.Domain.LearningPaths;

public sealed class LearningPathCourse
{
    public Guid LearningPathId { get; private set; }
    public LearningPath LearningPath { get; private set; } = default!;

    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = default!;

    public int Order { get; private set; }
    public bool IsRequired { get; private set; }

    private LearningPathCourse() { }

    private LearningPathCourse(Guid learningPathId, Guid courseId, int order, bool isRequired)
    {
        LearningPathId = learningPathId;
        CourseId = courseId;
        Order = order;
        IsRequired = isRequired;
    }

    public static Result<LearningPathCourse> Create(Guid learningPathId, Guid courseId, int order, bool isRequired = true)
    {
        if (learningPathId == Guid.Empty)
        {
            return Error.Validation("LearningPathCourse.PathIdRequired", "Learning Path ID is required.");
        }

        if (courseId == Guid.Empty)
        {
            return Error.Validation("LearningPathCourse.CourseIdRequired", "Course ID is required.");
        }

        if (order <= 0)
        {
            return Error.Validation("LearningPathCourse.InvalidOrder", "Order must be greater than 0.");
        }

        return new LearningPathCourse(learningPathId, courseId, order, isRequired);
    }

    internal void SetOrder(int order)
    {
        Order = order;
    }
}
