using LearnHub.Application.Features.Enrollments.Dtos;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Application.common.Interfaces;

public interface ICourseAccessService
{
    Task<Result<CourseAccessResult>> EvaluateAccessAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
    Task<Result<Updated>> SynchronizeUserEnrollmentsAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<Result<Guid>> EnsureEnrollmentForCourseAccessAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
    Task<Result<Updated>> ProcessOrderPaymentSucceededAsync(Guid orderId, CancellationToken cancellationToken = default);
}
