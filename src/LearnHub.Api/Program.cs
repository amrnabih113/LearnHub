using System.Text.Json.Serialization;
using Hangfire;
using LearnHub.Application;
using LearnHub.Infrastructure;
using LearnHub.Infrastructure.Data;
using Microsoft.OpenApi;

namespace LearnHub.Api;

public static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;

        builder.Services
        .AddApplication();
        builder.Services
        .AddInfrastructure(builder.Configuration);

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddEndpointsApiExplorer();

        var bearerScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT Bearer token format: Bearer {your token}"
        };

        var cookieScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Cookie,
            Name = "accessToken",
            Description = "Authentication cookie"
        };

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", bearerScheme);
            options.AddSecurityDefinition("cookieAuth", cookieScheme);

            options.AddSecurityRequirement((document) => new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference("Bearer"), new List<string>() },
                { new OpenApiSecuritySchemeReference("cookieAuth"), new List<string>() }
            });
        });




        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<AppDbContextInitializar>();
            await initializer.InitializeAsync();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}
