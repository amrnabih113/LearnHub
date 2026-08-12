using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Commands.DeleteTag;

public sealed record DeleteTagCommand(Guid Id) : IRequest<Result<Deleted>>;
