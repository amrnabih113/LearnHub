using LearnHub.Application.Features.Certificates.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Certificates.Queries.GetStudentCertificates;

public sealed record GetStudentCertificatesQuery(Guid StudentId) : IRequest<Result<IReadOnlyList<CertificateDto>>>;
