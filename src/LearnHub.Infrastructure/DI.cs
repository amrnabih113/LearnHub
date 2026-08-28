using System.Text;

using LearnHub.Application.Common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Courses.Options;

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
using Hangfire;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using LearnHub.Infrastructure.Storage;
using Microsoft.AspNetCore.Http;

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

        services.AddScoped<AppDbContextInitializar>();




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
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/problem+json";

                        await context.Response.WriteAsJsonAsync(
                            new
                            {
                                title = "Unauthorized",
                                detail = "Authentication token is missing or invalid.",
                                status = 401
                            });
                    }
                }; options.Events.OnForbidden = async context =>
{
    context.Response.StatusCode = 403;
    context.Response.ContentType = "application/problem+json";

    await context.Response.WriteAsJsonAsync(
        new
        {
            title = "Forbidden",
            detail = "You do not have permission to access this resource.",
            status = 403
        });
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
        // Background Jobs
        // ============================

        services.AddHangfire(config =>
    {
        config.UseSqlServerStorage(
            configuration.GetConnectionString("DefaultConnection"));
    });


        services.AddHangfireServer();

        // ============================
        // Notifications
        // ============================

        services.Configure<EmailSettings>(
      configuration.GetSection(
          EmailSettings.SectionName));


        services.AddScoped<IEmailService, MailService>();


        services.AddTransient<IEmailJob, EmailJob>();

        services.AddScoped<
            IBackgroundJobService,
            HangfireBackgroundJobService>();


        services.AddScoped<IEmailTemplateService, EmailTemplateService>();


        // ============================
        // Storage
        // ============================


        services.Configure<CloudinarySettings>(
            configuration.GetSection(CloudinarySettings.SectionName));

        services.AddSingleton(sp =>
        {
            var settings = sp
                .GetRequiredService<IOptions<CloudinarySettings>>()
                .Value;

            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret);

            return new Cloudinary(account);
        });

        services.Configure<FileStorageOptions>(
            configuration.GetSection(FileStorageOptions.SectionName));

        services.Configure<CourseThumbnailOptions>(
            configuration.GetSection(CourseThumbnailOptions.SectionName));

        services.AddScoped<IFileStorageService, FileStorageService>();

        // Certificate Options & Generator
        services.Configure<LearnHub.Application.common.Options.CertificateOptions>(
            configuration.GetSection(LearnHub.Application.common.Options.CertificateOptions.SectionName));
        services.AddScoped<ICertificateGenerator, LearnHub.Infrastructure.Services.CertificateGenerator>();

        // ============================
        // Caching
        // ============================

        // ============================
        // Course Access Service
        // ============================

        services.AddScoped<ICourseAccessService, LearnHub.Infrastructure.Services.CourseAccessService>();

        // ============================
        // Payments & Stripe
        // ============================

        services.Configure<LearnHub.Infrastructure.Services.StripeSettings>(
            configuration.GetSection(LearnHub.Infrastructure.Services.StripeSettings.SectionName));

        services.AddScoped<IStripeService, LearnHub.Infrastructure.Services.StripeService>();
        services.AddScoped<IPaymentGatewayService>(sp => sp.GetRequiredService<IStripeService>());
        services.AddScoped<IStripeWebhookService, LearnHub.Infrastructure.Services.StripeWebhookService>();

        return services;


    }
}