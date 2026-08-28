using FluentAssertions;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Options;
using LearnHub.Application.Features.Certificates.Commands.IssueCertificate;
using LearnHub.Application.Features.Certificates.Commands.RevokeCertificate;
using LearnHub.Application.Features.Certificates.Queries.VerifyCertificate;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Enrollments;
using LearnHub.Domain.Enrollments.Certificates;
using LearnHub.Domain.Enrollments.Enums;
using LearnHub.Domain.Identity;
using LearnHub.Infrastructure.Data;
using LearnHub.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LearnHub.UnitTests;

public class CertificateTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void CreateCertificate_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var id = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var code = "CERT-2026-TEST1234";

        // Act
        var result = Certificate.Create(
            id, enrollmentId, studentId, courseId, code,
            "John Doe", "C# Mastery", "Jane Smith");

        // Assert
        result.IsSuccess.Should().BeTrue();
        var cert = result.Value;
        cert.Id.Should().Be(id);
        cert.Code.Should().Be(code);
        cert.StudentName.Should().Be("John Doe");
        cert.CourseName.Should().Be("C# Mastery");
        cert.InstructorName.Should().Be("Jane Smith");
        cert.IsRevoked.Should().BeFalse();
        cert.RevocationReason.Should().BeNull();
    }

    [Fact]
    public void RevokeCertificate_WithValidReason_ShouldMarkAsRevoked()
    {
        // Arrange
        var cert = Certificate.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "CERT-REVOKE-1", "John Doe", "C#", "Instructor").Value;

        // Act
        var result = cert.Revoke("Issued by mistake");

        // Assert
        result.IsSuccess.Should().BeTrue();
        cert.IsRevoked.Should().BeTrue();
        cert.RevocationReason.Should().Be("Issued by mistake");
        cert.RevokedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void RevokeCertificate_AlreadyRevoked_ShouldReturnError()
    {
        // Arrange
        var cert = Certificate.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "CERT-REVOKE-2", "John Doe", "C#", "Instructor").Value;
        cert.Revoke("Reason 1");

        // Act
        var result = cert.Revoke("Reason 2");

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("DomainError.Certificate.AlreadyRevoked");
    }

    [Fact]
    public async Task GeneratePdfAsync_ShouldReturnValidPdfByteArray()
    {
        // Arrange
        var options = Options.Create(new CertificateOptions
        {
            OrganizationName = "LearnHub Platform",
            VerificationBaseUrl = "https://learnhub.org/verify"
        });
        var generator = new CertificateGenerator(options);
        var model = new CertificatePdfModel(
            "CERT-2026-PDFTEST",
            "Alice Smith",
            "Advanced Clean Architecture",
            "Bob Instructor",
            DateTimeOffset.UtcNow,
            "https://learnhub.org/verify/CERT-2026-PDFTEST");

        // Act
        var result = await generator.GeneratePdfAsync(model);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Value.Take(8).ToArray());
        pdfHeader.Should().StartWith("%PDF-1.4");
    }

    [Fact]
    public async Task VerifyCertificate_WhenValid_ShouldReturnValidDto()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var cert = Certificate.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "CERT-VERIFY-1", "Student A", "Course A", "Instructor A").Value;

        dbContext.Certificates.Add(cert);
        await dbContext.SaveChangesAsync();

        var handler = new VerifyCertificateQueryHandler(dbContext);

        // Act
        var result = await handler.Handle(new VerifyCertificateQuery("CERT-VERIFY-1"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsValid.Should().BeTrue();
        result.Value.Status.Should().Be("Valid");
        result.Value.StudentName.Should().Be("Student A");
    }

    [Fact]
    public async Task VerifyCertificate_WhenRevoked_ShouldReturnRevokedDto()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var cert = Certificate.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "CERT-VERIFY-REVOKED", "Student B", "Course B", "Instructor B").Value;

        cert.Revoke("Refund processed");
        dbContext.Certificates.Add(cert);
        await dbContext.SaveChangesAsync();

        var handler = new VerifyCertificateQueryHandler(dbContext);

        // Act
        var result = await handler.Handle(new VerifyCertificateQuery("CERT-VERIFY-REVOKED"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsValid.Should().BeFalse();
        result.Value.Status.Should().Be("Revoked");
        result.Value.RevocationReason.Should().Be("Refund processed");
    }

    [Fact]
    public async Task RevokeCertificateCommandHandler_ShouldRevokeInDatabase()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var certId = Guid.NewGuid();
        var cert = Certificate.Create(
            certId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "CERT-CMD-REVOKE", "Student C", "Course C", "Instructor C").Value;

        dbContext.Certificates.Add(cert);
        await dbContext.SaveChangesAsync();

        var handler = new RevokeCertificateCommandHandler(dbContext);

        // Act
        var result = await handler.Handle(new RevokeCertificateCommand(certId, "Violation of terms"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var updatedCert = await dbContext.Certificates.FindAsync(certId);
        updatedCert!.IsRevoked.Should().BeTrue();
        updatedCert.RevocationReason.Should().Be("Violation of terms");
    }
}
