using System.Text;
using Hangfire;
using LearnHub.Application;
using LearnHub.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
namespace LearnHub.Api;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;

        builder.Services
        .AddApplication();
        builder.Services
        .AddInfrastructure(builder.Configuration);
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

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();



        var app = builder.Build();



        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }



        app.UseHttpsRedirection();


        app.UseAuthentication();

        app.UseAuthorization();


        app.MapControllers();


        app.Run();
    }
}