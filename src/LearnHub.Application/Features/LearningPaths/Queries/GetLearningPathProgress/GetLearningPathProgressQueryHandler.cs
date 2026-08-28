using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.LearningPaths.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Enrollments.Enums;
using LearnHub.Domain.LearningPaths;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.LearningPaths.Queries.GetLearningPathProgress;

public sealed class GetLearningPathProgressQueryHandler(IAppDbContext context)
    : IRequestHandler<GetLearningPathProgressQuery, Result<LearningPathProgressDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<LearningPathProgressDto>> Handle(
        GetLearningPathProgressQuery request,
        CancellationToken cancellationToken)
    {
        var path = await _context.LearningPaths
            .Include(lp => lp.Courses)
                .ThenInclude(lpc => lpc.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(lp => lp.Id == request.LearningPathId, cancellationToken);

        if (path is null)
        {
            return LearningPathErrors.NotFound;
        }

        var orderedCourses = path.Courses
            .OrderBy(lpc => lpc.Order)
            .ToList();

        if (orderedCourses.Count == 0)
        {
            return new LearningPathProgressDto(
                path.Id,
                path.Title,
                0,
                0,
                0m,
                null,
                null,
                null,
                null,
                true);
        }

        var pathCourseIds = orderedCourses.Select(lpc => lpc.CourseId).ToList();

        var studentEnrollments = await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == request.StudentId && pathCourseIds.Contains(e.CourseId))
            .ToListAsync(cancellationToken);

        int completedCount = 0;
        Guid? currentCourseId = null;
        string? currentCourseTitle = null;
        Guid? nextCourseId = null;
        string? nextCourseTitle = null;

        for (int i = 0; i < orderedCourses.Count; i++)
        {
            var lpc = orderedCourses[i];
            var enrollment = studentEnrollments.FirstOrDefault(e => e.CourseId == lpc.CourseId);

            bool isCompleted = enrollment is not null &&
                               (enrollment.Status == EnrollmentStatus.Completed || enrollment.ProgressPercentage >= 100m);

            if (isCompleted)
            {
                completedCount++;
            }
            else if (currentCourseId is null)
            {
                currentCourseId = lpc.CourseId;
                currentCourseTitle = lpc.Course?.Title;

                if (i + 1 < orderedCourses.Count)
                {
                    nextCourseId = orderedCourses[i + 1].CourseId;
                    nextCourseTitle = orderedCourses[i + 1].Course?.Title;
                }
            }
        }

        int totalCourses = orderedCourses.Count;
        decimal progressPercentage = totalCourses == 0
            ? 0m
            : decimal.Round((completedCount * 100m) / totalCourses, 2);

        bool isCompletedPath = completedCount == totalCourses;

        return new LearningPathProgressDto(
            LearningPathId: path.Id,
            PathTitle: path.Title,
            TotalCourses: totalCourses,
            CompletedCourses: completedCount,
            ProgressPercentage: progressPercentage,
            CurrentCourseId: currentCourseId,
            CurrentCourseTitle: currentCourseTitle,
            NextCourseId: nextCourseId,
            NextCourseTitle: nextCourseTitle,
            IsCompleted: isCompletedPath);
    }
}
