using LearnHub.Application.Features.Student.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Student.Queries.GetStudentProfile;

public sealed record GetStudentProfileQuery(Guid StudentId)
    : IRequest<Result<StudentProfileDto>>;
