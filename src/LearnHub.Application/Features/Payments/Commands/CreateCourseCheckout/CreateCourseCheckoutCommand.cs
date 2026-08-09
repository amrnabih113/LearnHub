using LearnHub.Application.Features.Payments.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Payments.Commands.CreateCourseCheckout;

public sealed record CreateCourseCheckoutCommand(
    Guid StudentId,
    Guid CourseId,
    string SuccessUrl,
    string CancelUrl) : IRequest<Result<CheckoutSessionDto>>;
