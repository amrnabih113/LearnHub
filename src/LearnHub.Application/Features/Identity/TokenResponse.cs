using System.Text.Json.Serialization;

namespace LearnHub.Application.Features.Identity;

public record class TokenResponse
{
    public string? AccessToken { get; set; }

    [JsonIgnore]
    public string? RefreshToken { get; set; }

    public DateTime ExpiresOnUtc { get; set; }

    [JsonIgnore]
    public DateTimeOffset RefreshTokenExpiresOnUtc { get; set; }
}