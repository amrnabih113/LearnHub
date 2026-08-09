namespace LearnHub.Application.Features.Payments.Dtos;

public sealed record CheckoutSessionDto(
    string SessionId,
    string CheckoutUrl);
