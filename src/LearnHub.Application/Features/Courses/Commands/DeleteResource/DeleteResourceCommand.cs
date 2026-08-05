using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.DeleteResource;

public sealed record DeleteResourceCommand(Guid ResourceId) : IRequest<Result<Deleted>>;