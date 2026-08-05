using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.CreateSection;

public sealed record CreateSectionCommand(
    Guid CourseId,
    string Title,
    string Description,
    int Order) : IRequest<Result<Guid>>;