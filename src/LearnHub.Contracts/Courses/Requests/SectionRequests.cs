namespace LearnHub.Contracts.Courses.Requests;

public sealed record SectionOrderItemRequest(
    Guid SectionId,
    int Order);

public sealed record ReorderSectionsRequest(
    IReadOnlyList<SectionOrderItemRequest> Items);
