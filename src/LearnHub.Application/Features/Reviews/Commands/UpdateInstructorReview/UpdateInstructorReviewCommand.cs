using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Reviews.Commands.UpdateInstructorReview;

public sealed record UpdateInstructorReviewCommand(
    Guid ReviewId,
    Guid StudentId,
    int Rating,
    string Comment) : IRequest<Result<InstructorReviewDto>>;
