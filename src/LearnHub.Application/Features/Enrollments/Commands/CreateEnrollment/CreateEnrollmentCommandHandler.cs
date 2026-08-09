using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Enrollments.Commands.CreateEnrollment;

public sealed class CreateEnrollmentCommandHandler(ICourseAccessService courseAccessService)
    : IRequestHandler<CreateEnrollmentCommand, Result<Guid>>
{
    private readonly ICourseAccessService _courseAccessService = courseAccessService;

    public async Task<Result<Guid>> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
    {
        return await _courseAccessService.EnsureEnrollmentForCourseAccessAsync(
            request.StudentId,
            request.CourseId,
            cancellationToken);
    }
}
