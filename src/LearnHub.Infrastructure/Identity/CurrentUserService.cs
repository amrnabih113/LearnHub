using System.Security.Claims;

using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Domain.Identity;

using Microsoft.AspNetCore.Http;



namespace LearnHub.Infrastructure.Identity;


public sealed class CurrentUserService
    : ICurrentUserService
{

    private readonly IHttpContextAccessor _accessor;



    public CurrentUserService(
        IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }



    private ClaimsPrincipal? User =>
        _accessor.HttpContext?.User;



    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;



    public Guid? UserId
    {
        get
        {
            var id =
                User?
                .FindFirstValue(
                    ClaimTypes.NameIdentifier);


            return Guid.TryParse(id, out var guid)
                ? guid
                : null;
        }
    }



    public string? Email =>
        User?
        .FindFirstValue(
            ClaimTypes.Email);

    public IReadOnlyCollection<Role> Roles
    {
        get
        {
            var roles =
                User?
                .FindAll(ClaimTypes.Role)
                .Select(x => x.Value)
                .Where(x => Enum.TryParse<Role>(x, out _))
                .Select(Enum.Parse<Role>)
                .ToList();

            return roles ?? [];
        }
    }
    public bool IsInRole(Role role)
    {
        return User?.IsInRole(role.ToString()) == true;
    }
}