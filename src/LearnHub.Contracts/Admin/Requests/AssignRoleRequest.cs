using LearnHub.Domain.Identity;

namespace LearnHub.Contracts.Admin.Requests;

public sealed record AssignRoleRequest(
    Role Role);
