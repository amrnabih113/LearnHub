using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Identity;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace LearnHub.Infrastructure.Identity;

public sealed class TokenProvider : ITokenProvider
{
    private readonly IConfiguration _configuration;


    public TokenProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }



    public Task<Result<TokenResponse>> GenerateJwtTokenAsync(
        User user,
        CancellationToken ct = default)
    {
        var jwtSettings =
            _configuration.GetSection("JwtSettings");


        var expirationMinutes =
            int.Parse(
                jwtSettings["ExpirationMinutes"]
                ?? throw new InvalidOperationException(
                    "JWT ExpirationMinutes missing"));


        var expiresOnUtc =
            DateTime.UtcNow.AddMinutes(expirationMinutes);



        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),


            new(
                ClaimTypes.Email,
                user.Email),


            new(
                ClaimTypes.Name,
                user.FullName),


            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };



        foreach (var role in user.Roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role.Role.ToString()));
        }



        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwtSettings["Secret"]
                    ?? throw new InvalidOperationException(
                        "JWT Secret missing")));



        var token =
            new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiresOnUtc,
                signingCredentials:
                    new SigningCredentials(
                        key,
                        SecurityAlgorithms.HmacSha256));



        var accessToken =
            new JwtSecurityTokenHandler()
            .WriteToken(token);



        return Task.FromResult<Result<TokenResponse>>(
            new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = null,
                ExpiresOnUtc = expiresOnUtc,
                RefreshTokenExpiresOnUtc = default
            });
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public DateTimeOffset GetRefreshTokenExpiresOnUtc()
        => DateTimeOffset.UtcNow.AddDays(7);



    public ClaimsPrincipal? GetPrincipalFromExpiredToken(
        string token)
    {
        var jwtSettings =
            _configuration.GetSection("JwtSettings");


        var parameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,

                ValidateLifetime = false,

                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings["Issuer"],

                ValidAudience = jwtSettings["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings["Secret"]!))
            };


        try
        {
            return new JwtSecurityTokenHandler()
                .ValidateToken(
                    token,
                    parameters,
                    out _);
        }
        catch
        {
            return null;
        }
    }
}