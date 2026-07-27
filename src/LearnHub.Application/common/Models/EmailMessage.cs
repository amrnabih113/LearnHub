namespace LearnHub.Application.Common.Models;

public sealed record EmailMessage(
    string To,
    string Subject,
    EmailTemplate Template,
    Dictionary<string, string> Data);
