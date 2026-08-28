using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Student.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Student.Queries.GetStudentStatistics;

public sealed class GetStudentStatisticsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetStudentStatisticsQuery, Result<StudentStatisticsDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<StudentStatisticsDto>> Handle(
        GetStudentStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var studentEnrollmentIds = await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == request.StudentId)
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);

        int enrolledCoursesCount = studentEnrollmentIds.Count;

        var certificatesCount = await _context.Certificates
            .AsNoTracking()
            .CountAsync(c => c.StudentId == request.StudentId && !c.IsRevoked, cancellationToken);

        if (studentEnrollmentIds.Count == 0)
        {
            var emptyWeekly = GetEmptyWeeklyActivity(DateTime.UtcNow);
            return new StudentStatisticsDto(0, 0, 0, 0, certificatesCount, null, emptyWeekly);
        }

        var lessonProgressRecords = await _context.LessonProgresses
            .AsNoTracking()
            .Where(lp => studentEnrollmentIds.Contains(lp.EnrollmentId))
            .ToListAsync(cancellationToken);

        if (lessonProgressRecords.Count == 0)
        {
            var emptyWeekly = GetEmptyWeeklyActivity(DateTime.UtcNow);
            return new StudentStatisticsDto(0, enrolledCoursesCount, 0, 0, certificatesCount, null, emptyWeekly);
        }

        var now = DateTime.UtcNow;
        var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
        if (now.DayOfWeek == DayOfWeek.Sunday)
        {
            startOfWeek = startOfWeek.AddDays(-7);
        }

        // Learning time this week (minutes)
        var weeklyProgress = lessonProgressRecords
            .Where(lp => (lp.UpdatedAtUtc ?? lp.CreatedAtUtc) >= startOfWeek)
            .ToList();

        int learningTimeThisWeekMinutes = (int)Math.Round(weeklyProgress.Sum(lp => lp.WatchDurationSeconds) / 60.0);

        // Calculate Weekly Activity Chart
        var weeklyActivityList = new List<DailyActivityDto>();
        for (int i = 0; i < 7; i++)
        {
            var dayDate = startOfWeek.AddDays(i).Date;
            var dayWatchSeconds = lessonProgressRecords
                .Where(lp => (lp.UpdatedAtUtc ?? lp.CreatedAtUtc).Date == dayDate)
                .Sum(lp => lp.WatchDurationSeconds);

            weeklyActivityList.Add(new DailyActivityDto(
                DayOfWeek: dayDate.DayOfWeek.ToString(),
                MinutesLearned: (int)Math.Round(dayWatchSeconds / 60.0),
                Date: dayDate));
        }

        // Calculate Streak from actual learning activity dates
        var activityDates = lessonProgressRecords
            .Select(lp => (lp.UpdatedAtUtc ?? lp.CreatedAtUtc).Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        var lastActivityUtc = lessonProgressRecords.Max(lp => lp.UpdatedAtUtc ?? lp.CreatedAtUtc);

        int currentStreak = 0;
        int longestStreak = 0;
        int tempStreak = 0;

        if (activityDates.Count > 0)
        {
            var today = now.Date;
            var yesterday = today.AddDays(-1);

            // Current streak check
            if (activityDates.Contains(today) || activityDates.Contains(yesterday))
            {
                var checkDate = activityDates.Contains(today) ? today : yesterday;
                while (activityDates.Contains(checkDate))
                {
                    currentStreak++;
                    checkDate = checkDate.AddDays(-1);
                }
            }

            // Longest streak check
            var sortedAsc = activityDates.OrderBy(d => d).ToList();
            tempStreak = 1;
            longestStreak = 1;

            for (int i = 1; i < sortedAsc.Count; i++)
            {
                if (sortedAsc[i] == sortedAsc[i - 1].AddDays(1))
                {
                    tempStreak++;
                }
                else
                {
                    tempStreak = 1;
                }

                if (tempStreak > longestStreak)
                {
                    longestStreak = tempStreak;
                }
            }
        }

        return new StudentStatisticsDto(
            LearningTimeThisWeekMinutes: learningTimeThisWeekMinutes,
            EnrolledCourses: enrolledCoursesCount,
            CurrentStreakDays: currentStreak,
            LongestStreakDays: Math.Max(currentStreak, longestStreak),
            Certificates: certificatesCount,
            LastLearningActivityUtc: lastActivityUtc,
            WeeklyActivity: weeklyActivityList);
    }

    private static List<DailyActivityDto> GetEmptyWeeklyActivity(DateTime now)
    {
        var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
        var list = new List<DailyActivityDto>();
        for (int i = 0; i < 7; i++)
        {
            var d = startOfWeek.AddDays(i).Date;
            list.Add(new DailyActivityDto(d.DayOfWeek.ToString(), 0, d));
        }
        return list;
    }
}
