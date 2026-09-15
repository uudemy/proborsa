using System.Net.Http;
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

        services.AddSingleton<HttpClient>(_ =>
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "StockPro/1.0");

            return client;
        });

        services.AddSingleton<YahooMarketDataProvider>();

        services.AddSingleton<IMarketDataProvider>(
            provider =>
                provider.GetRequiredService<
                    YahooMarketDataProvider>());

        services.AddSingleton<MockMarketDataProvider>();

        services.AddSingleton<MockStreamingMarketDataProvider>();

        services.AddSingleton<IStreamingMarketDataProvider>(
            provider =>
                provider.GetRequiredService<
                    MockStreamingMarketDataProvider>());

        services.AddSingleton<MockNewsProvider>();

        services.AddSingleton<IMarketDataService, MarketDataService>();

        services.AddSingleton<INewsService, NewsService>();

        return services;
    }
}