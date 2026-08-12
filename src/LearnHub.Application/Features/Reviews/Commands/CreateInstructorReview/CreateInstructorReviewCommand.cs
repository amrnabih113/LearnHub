using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Reviews.Commands.CreateInstructorReview;

public sealed record CreateInstructorReviewCommand(
    Guid InstructorId,
    Guid StudentId,
    int Rating,
    string Comment,
    Guid? CourseId = null) : IRequest<Result<InstructorReviewDto>>;
