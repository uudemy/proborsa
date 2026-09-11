using StockPro.Application.Interfaces;
using StockPro.Domain.Entities;
using StockPro.Infrastructure.News;

namespace StockPro.Infrastructure.Services;

public sealed class NewsService : INewsService
{
    private readonly MockNewsProvider _provider;

    public NewsService(MockNewsProvider provider)
    {
        _provider = provider;
    }

    public Task<IReadOnlyList<NewsItem>> GetLatestAsync(
        int count,
        CancellationToken cancellationToken)
    {
        return _provider.GetLatestAsync(
            count,
            cancellationToken);
    }

    public Task<IReadOnlyList<NewsItem>> GetBySymbolAsync(
        string symbol,
        int count,
        CancellationToken cancellationToken)
    {
        return _provider.GetBySymbolAsync(
            symbol,
            count,
            cancellationToken);
    }

    public Task<NewsItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return _provider.GetByIdAsync(
            id,
            cancellationToken);
    }
}