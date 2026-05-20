using System.Security.Claims;
using LearnHub.Application.Features.Identity.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;

namespace LearnHub.Application.Features.Identity.Mappers;

public static class UserDtoMapper

{
    public static Result<UserDto> ToDto(this User user) => new UserDto(
        Id: user.Id,
        FirstName: user.FirstName,
        LastName: user.LastName,
        FullName: $"{user.FirstName} {user.LastName}",
        Email: user.Email,
        PhoneNumber: user.PhoneNumber,
        Role: user.Roles.FirstOrDefault(),
        ImageUrl: user.ImageUrl,
        DateOfBirth: user.DateOfBirth,
        Bio: user.Bio,
        Country: user.Country);
}