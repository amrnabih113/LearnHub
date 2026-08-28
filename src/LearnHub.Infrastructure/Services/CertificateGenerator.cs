using System.Text;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Application.common.Options;
using Microsoft.Extensions.Options;

namespace LearnHub.Infrastructure.Services;

public sealed class CertificateGenerator(IOptions<CertificateOptions> options) : ICertificateGenerator
{
    private readonly CertificateOptions _options = options.Value;

    public Task<Result<byte[]>> GeneratePdfAsync(
        CertificatePdfModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var pdfBytes = GenerateCertificatePdfBytes(model);
            return Task.FromResult<Result<byte[]>>(pdfBytes);
        }
        catch (Exception ex)
        {
            return Task.FromResult<Result<byte[]>>(
                Error.Failure("Certificate.PdfGenerationFailed", $"PDF generation failed: {ex.Message}"));
        }
    }

    private byte[] GenerateCertificatePdfBytes(CertificatePdfModel model)
    {
        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, Encoding.ASCII, leaveOpen: true);
        var offsets = new List<long>();

        var issuedDateStr = model.IssuedAtUtc.ToString("MMMM dd, yyyy");
        var orgName = string.IsNullOrWhiteSpace(_options.OrganizationName) ? "LearnHub Platform" : _options.OrganizationName;

        writer.Write("%PDF-1.4\n");
        writer.Flush();

        // Obj 1: Catalog
        offsets.Add(ms.Position);
        writer.Write("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");
        writer.Flush();

        // Obj 2: Pages
        offsets.Add(ms.Position);
        writer.Write("2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n");
        writer.Flush();

        // Obj 3: Page
        offsets.Add(ms.Position);
        writer.Write("3 0 obj\n<< /Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [0 0 842 595] /Contents 5 0 R >>\nendobj\n");
        writer.Flush();

        // Obj 4: Fonts
        offsets.Add(ms.Position);
        writer.Write("4 0 obj\n<< /Font << /F1 << /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >> /F2 << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> /F3 << /Type /Font /Subtype /Type1 /BaseFont /Times-BoldItalic >> >> >>\nendobj\n");
        writer.Flush();

        // Content stream containing vector graphics & text layout
        var contentSb = new StringBuilder();

        // Background & Gold Ornate Border
        contentSb.AppendLine("0.98 0.98 0.96 rg 0 0 842 595 re f"); // Warm ivory background
        contentSb.AppendLine("0.84 0.68 0.22 RG 12 w 20 20 802 555 re s"); // Gold outer border
        contentSb.AppendLine("0.11 0.17 0.29 RG 2 w 30 30 782 535 re s"); // Navy inner accent border

        // Header Title
        contentSb.AppendLine("BT");
        contentSb.AppendLine("/F1 28 Tf 0.11 0.17 0.29 rg 421 510 Td (" + EscapePdfText(orgName.ToUpperInvariant()) + ") Tj ET");
        contentSb.AppendLine("BT");
        contentSb.AppendLine("/F3 36 Tf 0.84 0.68 0.22 rg 421 455 Td (CERTIFICATE OF COMPLETION) Tj ET");
        contentSb.AppendLine("BT");
        contentSb.AppendLine("/F2 14 Tf 0.3 0.3 0.3 rg 421 415 Td (This is proudly presented to) Tj ET");

        // Student Name
        contentSb.AppendLine("BT");
        contentSb.AppendLine("/F1 32 Tf 0.11 0.17 0.29 rg 421 365 Td (" + EscapePdfText(model.StudentName) + ") Tj ET");
        contentSb.AppendLine("0.84 0.68 0.22 RG 2 w 221 350 m 621 350 l S"); // Underline

        // Course Info
        contentSb.AppendLine("BT");
        contentSb.AppendLine("/F2 14 Tf 0.3 0.3 0.3 rg 421 315 Td (for successfully completing the course) Tj ET");
        contentSb.AppendLine("BT");
        contentSb.AppendLine("/F1 24 Tf 0.11 0.17 0.29 rg 421 275 Td (" + EscapePdfText(model.CourseTitle) + ") Tj ET");

        // Signatures & Details Footer
        contentSb.AppendLine("BT");
        contentSb.AppendLine("/F2 12 Tf 0.3 0.3 0.3 rg 150 170 Td (Instructor: " + EscapePdfText(model.InstructorName) + ") Tj ET");
        contentSb.AppendLine("0.3 0.3 0.3 RG 1 w 150 160 m 300 160 l S");

        contentSb.AppendLine("BT");
        contentSb.AppendLine("/F2 12 Tf 0.3 0.3 0.3 rg 550 170 Td (Issue Date: " + EscapePdfText(issuedDateStr) + ") Tj ET");
        contentSb.AppendLine("0.3 0.3 0.3 RG 1 w 550 160 m 700 160 l S");

        // Code & Verification Line
        contentSb.AppendLine("BT");
        contentSb.AppendLine("/F2 10 Tf 0.5 0.5 0.5 rg 421 90 Td (Certificate Code: " + EscapePdfText(model.CertificateCode) + ") Tj ET");
        contentSb.AppendLine("BT");
        contentSb.AppendLine("/F2 9 Tf 0.2 0.4 0.8 rg 421 72 Td (Verify at: " + EscapePdfText(model.VerificationUrl) + ") Tj ET");

        var contentBytes = Encoding.ASCII.GetBytes(contentSb.ToString());

        // Obj 5: Content Object
        offsets.Add(ms.Position);
        writer.Write("5 0 obj\n<< /Length " + contentBytes.Length + " >>\nstream\n");
        writer.Flush();
        ms.Write(contentBytes, 0, contentBytes.Length);
        writer.Write("\nendstream\nendobj\n");
        writer.Flush();

        // Cross-reference table & trailer
        var startXref = ms.Position;
        writer.Write("xref\n0 6\n0000000000 65535 f \n");
        foreach (var offset in offsets)
        {
            writer.Write($"{offset:D10} 00000 n \n");
        }

        writer.Write("trailer\n<< /Size 6 /Root 1 0 R >>\nstartxref\n");
        writer.Write($"{startXref}\n%%EOF\n");
        writer.Flush();

        return ms.ToArray();
    }

    private static string EscapePdfText(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
    }
}
