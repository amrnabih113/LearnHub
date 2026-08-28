using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Certificates.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetCertificatesAdmin;

public sealed record GetCertificatesAdminQuery(
    string? Search = null,
    Guid? CourseId = null,
    Guid? StudentId = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<CertificateDto>>>;
