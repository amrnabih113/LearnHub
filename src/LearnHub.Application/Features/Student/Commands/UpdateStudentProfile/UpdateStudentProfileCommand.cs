using LearnHub.Application.Features.Student.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Student.Commands.UpdateStudentProfile;

public sealed record UpdateStudentProfileCommand(
    Guid StudentId,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateOnly? DateOfBirth,
    string? Bio,
    string? Country
) : IRequest<Result<StudentProfileDto>>;
