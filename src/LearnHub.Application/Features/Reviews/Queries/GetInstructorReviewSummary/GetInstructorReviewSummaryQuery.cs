using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Reviews.Queries.GetInstructorReviewSummary;

public sealed record GetInstructorReviewSummaryQuery(Guid InstructorId) : IRequest<Result<ReviewSummaryDto>>;
