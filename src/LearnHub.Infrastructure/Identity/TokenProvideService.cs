using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces;
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


        var expiresOnUtc =
            DateTime.UtcNow.AddMinutes(
                int.Parse(jwtSettings["ExpirationMinutes"]!));



        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),


            new(
                JwtRegisteredClaimNames.Email,
                user.Email),


            new(
                ClaimTypes.Name,
                user.FullName)
        };


        foreach (var userRole in user.Roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    userRole.Role.ToString()));
        }



        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                jwtSettings["Secret"]!));


        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);



        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresOnUtc,
            signingCredentials: credentials);



        var accessToken =
            new JwtSecurityTokenHandler()
            .WriteToken(token);



        var response = new TokenResponse
        {
            AccessToken = accessToken,

            // implement refresh token later
            RefreshToken = null,

            ExpiresOnUtc = expiresOnUtc
        };


        return Task.FromResult<Result<TokenResponse>>(response);
    
    }





    public ClaimsPrincipal? GetPrincipalFromExpiredToken(
        string token)
    {
        var jwtSettings =
            _configuration.GetSection("JwtSettings");


        var validationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = false,

                ValidateIssuerSigningKey = true,


                ValidIssuer =
                    jwtSettings["Issuer"],


                ValidAudience =
                    jwtSettings["Audience"],


                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings["Secret"]!))
            };


        var tokenHandler =
            new JwtSecurityTokenHandler();


        try
        {
            var principal =
                tokenHandler.ValidateToken(
                    token,
                    validationParameters,
                    out _);


            return principal;
        }
        catch
        {
            return null;
        }
    }
}