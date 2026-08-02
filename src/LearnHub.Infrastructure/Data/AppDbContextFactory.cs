using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LearnHub.Infrastructure.Data;

public sealed class AppDbContextFactory
    : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "../LearnHub.Api"))
            .AddJsonFile(
                "appsettings.json",
                optional: false)
            .Build();


        var optionsBuilder =
            new DbContextOptionsBuilder<AppDbContext>();


        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString(
                "DefaultConnection"));



        var services = new ServiceCollection();


        services.AddLogging();


        services.AddMediatR(
            cfg =>
            {
                cfg.RegisterServicesFromAssembly(
                    typeof(AppDbContext).Assembly);
            });



        var provider =
            services.BuildServiceProvider();



        var mediator =
            provider.GetRequiredService<IMediator>();


        return new AppDbContext(
            optionsBuilder.Options,
            mediator);
    }
}