using System.Text;

using LearnHub.Application.Common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.common.Interfaces;

using LearnHub.Infrastructure.Identity;
using LearnHub.Infrastructure.Data;
using LearnHub.Infrastructure.Data.Interceptors;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using LearnHub.Infrastructure.Email;
using LearnHub.Infrastructure.BackgroundJobs;
using LearnHub.Infrastructure.Email.Templates;

namespace LearnHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        // ============================
        // Core Infrastructure Services
        // ============================

        services.AddSingleton(TimeProvider.System);


        // ============================
        // Database
        // ============================

        var connectionString =
            configuration.GetConnectionString(
                "DefaultConnection");


        ArgumentNullException.ThrowIfNull(connectionString);



        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();


        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.AddInterceptors(
                sp.GetServices<ISaveChangesInterceptor>());


            options.UseSqlServer(connectionString);
        });



        services.AddScoped<IAppDbContext>(
            provider =>
                provider.GetRequiredService<AppDbContext>());



        // ============================
        // Authentication
        // ============================

        services.AddHttpContextAccessor();



        var jwtSettings =
            configuration.GetSection("JwtSettings");


        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;


                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })

            .AddJwtBearer(options =>
            {

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {

                        ValidateIssuer = true,

                        ValidateAudience = true,

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,


                        ClockSkew = TimeSpan.Zero,


                        ValidIssuer =
                            jwtSettings["Issuer"],


                        ValidAudience =
                            jwtSettings["Audience"],


                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtSettings["Secret"]!))
                    };
            });



        // ============================
        // Identity Services
        // ============================


        services.AddScoped<
            ICurrentUserService,
            CurrentUserService>();


        services.AddScoped<
            ITokenProvider,
            TokenProvider>();


        services.AddScoped<
            IOtpProvider,
            OtpProvider>();


        services.AddScoped<
            IPasswordHasher,
            PasswordHasherService>();



        // ============================
        // Authorization
        // ============================

        services.AddAuthorization();



        // ============================
        // Notifications
        // ============================

        services.Configure<EmailSettings>(
      configuration.GetSection(
          EmailSettings.SectionName));


        services.AddScoped<IEmailService, MailService>();


        services.AddSingleton<IEmailQueue, BackgroundEmailQueue>();


        services.AddScoped<IEmailTemplateService, EmailTemplateService>();


        services.AddHostedService<EmailBackgroundService>();
        // ============================
        // Storage
        // ============================

        // later:
        // services.AddScoped<IFileStorageService, CloudinaryFileStorageService>();


        // ============================
        // Caching
        // ============================

        // later:
        // services.AddHybridCache();



        return services;
    }
}