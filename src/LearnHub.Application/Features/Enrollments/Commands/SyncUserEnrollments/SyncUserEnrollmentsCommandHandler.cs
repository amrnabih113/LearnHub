using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Enrollments.Commands.SyncUserEnrollments;

public sealed class SyncUserEnrollmentsCommandHandler(ICourseAccessService courseAccessService)
    : IRequestHandler<SyncUserEnrollmentsCommand, Result<Updated>>
{
    private readonly ICourseAccessService _courseAccessService = courseAccessService;

    public async Task<Result<Updated>> Handle(SyncUserEnrollmentsCommand request, CancellationToken cancellationToken)
    {
        return await _courseAccessService.SynchronizeUserEnrollmentsAsync(request.StudentId, cancellationToken);
    }
}
