using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Enrollments.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Enrollments.Queries.GetCourseAccess;

public sealed class GetCourseAccessQueryHandler(ICourseAccessService courseAccessService)
    : IRequestHandler<GetCourseAccessQuery, Result<CourseAccessResult>>
{
    private readonly ICourseAccessService _courseAccessService = courseAccessService;

    public async Task<Result<CourseAccessResult>> Handle(GetCourseAccessQuery request, CancellationToken cancellationToken)
    {
        return await _courseAccessService.EvaluateAccessAsync(request.StudentId, request.CourseId, cancellationToken);
    }
}
