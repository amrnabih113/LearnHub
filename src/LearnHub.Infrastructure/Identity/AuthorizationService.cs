using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace LearnHub.Infrastructure.Identity;


public sealed class AuthorizationService
    : Application.Common.Interfaces.IAuthorizationService
{

    private readonly IAuthorizationService _authorization;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public AuthorizationService(
        IAuthorizationService authorization,
        IHttpContextAccessor httpContextAccessor)
    {
        _authorization = authorization;
        _httpContextAccessor = httpContextAccessor;
    }



    public Task<bool> IsInRoleAsync(
        string userId,
        string role)
    {
        var user =
            _httpContextAccessor.HttpContext?.User;


        if (user is null)
            return Task.FromResult(false);


        return Task.FromResult(
            user.IsInRole(role));
    }



    public async Task<bool> AuthorizeAsync(
        string userId,
        string policyName)
    {
        var user =
            _httpContextAccessor.HttpContext?.User;


        if (user is null)
            return false;


        var result =
            await _authorization.AuthorizeAsync(
                user,
                policyName);


        return result.Succeeded;
    }
}