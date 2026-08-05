using LearnHub.Domain.Courses.Sections.Lessons.Resources;

namespace LearnHub.Contracts.Courses.Requests;

public sealed record CreateResourceRequest(
    string Name,
    string Url,
    ResourceType Type,
    long SizeInBytes);
