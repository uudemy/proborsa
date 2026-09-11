using Microsoft.Extensions.DependencyInjection;
using StockPro.Application.Interfaces;
using StockPro.Domain.Interfaces;
using StockPro.Infrastructure.Caching;
using StockPro.Infrastructure.MarketData;
using StockPro.Infrastructure.News;
using StockPro.Infrastructure.Repositories;
using StockPro.Infrastructure.Services;

namespace StockPro.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStockProInfrastructure(
        this IServiceCollection services)
    {
        services.AddSingleton<IMarketDataCache, InMemoryMarketDataCache>();

        services.AddSingleton<MockMarketDataProvider>();
        services.AddSingleton<IMarketDataProvider>(
            provider => provider.GetRequiredService<MockMarketDataProvider>());

        services.AddSingleton<MockStreamingMarketDataProvider>();
        services.AddSingleton<IStreamingMarketDataProvider>(
            provider => provider.GetRequiredService<MockStreamingMarketDataProvider>());

        services.AddSingleton<MockNewsProvider>();

        services.AddSingleton<IMarketDataService, MarketDataService>();
        services.AddSingleton<INewsService, NewsService>();

        return services;
    }
}