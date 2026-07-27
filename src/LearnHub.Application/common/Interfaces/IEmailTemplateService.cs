using LearnHub.Application.Common.Models;

namespace LearnHub.Application.Common.Interfaces;



public interface IEmailTemplateService
{
    string Render(
        EmailTemplate template,
        Dictionary<string, string> data);
}