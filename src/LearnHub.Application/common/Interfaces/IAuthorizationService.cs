namespace LearnHub.Application.Common.Interfaces.Authentication;

public interface IAuthorizationService
{
    Task<bool> IsInRoleAsync(
        string userId,
        string role);

    Task<bool> AuthorizeAsync(
        string userId,
        string policyName);
}