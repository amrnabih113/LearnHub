using LearnHub.Application.Features.Certificates.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Certificates.Commands.ReissueCertificate;

public sealed record ReissueCertificateCommand(Guid CertificateId)
    : IRequest<Result<CertificateDto>>;
