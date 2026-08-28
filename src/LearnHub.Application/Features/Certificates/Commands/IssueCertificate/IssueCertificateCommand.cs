using LearnHub.Application.Features.Certificates.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Certificates.Commands.IssueCertificate;

public sealed record IssueCertificateCommand(
    Guid StudentId,
    Guid CourseId) : IRequest<Result<CertificateDto>>;
