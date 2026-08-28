using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Enrollments.Certificates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Certificates.Commands.RevokeCertificate;

public sealed class RevokeCertificateCommandHandler(IAppDbContext context)
    : IRequestHandler<RevokeCertificateCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        RevokeCertificateCommand request,
        CancellationToken cancellationToken)
    {
        if (request.CertificateId == Guid.Empty)
        {
            return Error.Validation("Certificate.IdRequired", "Certificate ID is required.");
        }

        var cert = await _context.Certificates
            .FirstOrDefaultAsync(c => c.Id == request.CertificateId, cancellationToken);

        if (cert is null)
        {
            return CertificateErrors.CertificateNotFound;
        }

        var revokeResult = cert.Revoke(request.Reason);
        if (revokeResult.IsError)
        {
            return revokeResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
