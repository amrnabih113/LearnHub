using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Certificates.Commands.RevokeCertificate;

public sealed record RevokeCertificateCommand(
    Guid CertificateId,
    string Reason) : IRequest<Result<Updated>>;
