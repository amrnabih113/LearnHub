using FluentValidation;
using LearnHub.Application.common.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LearnHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddHybridCache();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        });

        services.AddSingleton<Features.Search.Services.ISearchQueryNormalizer, Features.Search.Services.SearchQueryNormalizer>();
        services.AddSingleton<Features.Search.Services.IFuzzyMatcher, Features.Search.Services.FuzzyMatcher>();
        services.AddSingleton<Features.Search.Services.ISynonymProvider, Features.Search.Services.SynonymProvider>();
        services.AddSingleton<Features.Search.Services.ISemanticSearchProvider, Features.Search.Services.NullSemanticSearchProvider>();
        services.AddSingleton<Features.Search.Services.ISearchRankingService, Features.Search.Services.SearchRankingService>();

        return services;
    }
}
