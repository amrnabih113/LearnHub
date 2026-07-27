using System.Text;
using Hangfire;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Services
    .AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,


                ValidIssuer =
                    configuration["JwtSettings:Issuer"],


                ValidAudience =
                    configuration["JwtSettings:Audience"],


                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            configuration["JwtSettings:Secret"]!))
            };
    });
var app = builder.Build();
app.UseHangfireDashboard();
app.MapGet("/", () => "Hello World!");

app.Run();
