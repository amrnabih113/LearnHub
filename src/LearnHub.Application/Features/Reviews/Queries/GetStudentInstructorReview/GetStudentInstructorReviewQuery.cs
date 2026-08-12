using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Reviews.Queries.GetStudentInstructorReview;

public sealed record GetStudentInstructorReviewQuery(
    Guid InstructorId,
    Guid StudentId) : IRequest<Result<InstructorReviewDto?>>;
