using LearnHub.Application.Features.Certificates.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Certificates.Queries.GetCertificateById;

public sealed record GetCertificateByIdQuery(
    Guid CertificateId,
    Guid StudentId,
    bool IsAdmin = false) : IRequest<Result<CertificateDto>>;
