using StockPro.Domain.Entities;

namespace StockPro.Application.Interfaces;

public interface INewsService
{
    Task<IReadOnlyList<NewsItem>> GetLatestAsync(
        int count,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<NewsItem>> GetBySymbolAsync(
        string symbol,
        int count,
        CancellationToken cancellationToken);

    Task<NewsItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}