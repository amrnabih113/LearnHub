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



    public Role? Role
    {
        get
        {
            var role =
                User?
                .FindFirstValue(
                    ClaimTypes.Role);


            if (Enum.TryParse<Role>(
                role,
                out var result))
            {
                return result;
            }


            return null;
        }
    }
}