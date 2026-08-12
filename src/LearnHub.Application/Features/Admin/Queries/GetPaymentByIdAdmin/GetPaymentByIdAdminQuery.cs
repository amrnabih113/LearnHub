using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetPaymentByIdAdmin;

public sealed record GetPaymentByIdAdminQuery(Guid Id) : IRequest<Result<PaymentAdminSummaryDto>>;
