using LearnHub.Application.Features.Certificates.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Certificates.Queries.VerifyCertificate;

public sealed record VerifyCertificateQuery(string Code) : IRequest<Result<CertificateVerificationDto>>;
