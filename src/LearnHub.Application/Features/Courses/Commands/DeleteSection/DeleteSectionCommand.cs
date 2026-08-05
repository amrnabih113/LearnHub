using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.DeleteSection;

public sealed record DeleteSectionCommand(Guid SectionId) : IRequest<Result<Deleted>>;