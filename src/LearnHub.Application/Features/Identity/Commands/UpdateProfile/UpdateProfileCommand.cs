using LearnHub.Application.Features.Identity.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Identity.Commands.UpdateProfile;



public sealed record UpdateProfileCommand(
Guid Id,
string FirstName,
string LastName,
string? Bio = null,
string? PhoneNumber = null,
string? Country = null,
DateOnly? DateOfBirth = null
) : IRequest<Result<Updated>>;