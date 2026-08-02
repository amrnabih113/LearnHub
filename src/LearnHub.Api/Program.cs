using Hangfire;
using LearnHub.Application;
using LearnHub.Infrastructure;
using Microsoft.OpenApi;
namespace LearnHub.Api;

public static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;

        builder.Services
        .AddApplication();
        builder.Services
        .AddInfrastructure(builder.Configuration);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Cookie,
                Name = "accessToken", // Your cookie name
                Description = "Authentication cookie"
            });


        });


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