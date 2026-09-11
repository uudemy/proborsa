using StockPro.Domain.Entities;
using StockPro.Domain.Enums;

namespace StockPro.Infrastructure.Repositories;

public interface IMarketDataCache
{
    Task<Quote?> GetQuoteAsync(
        string symbol,
        CancellationToken cancellationToken);

    Task SetQuoteAsync(
        string symbol,
        Quote quote,
        TimeSpan expiration,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Candle>?> GetHistoricalDataAsync(
        string symbol,
        TimeFrame timeframe,
        CancellationToken cancellationToken);

    Task SetHistoricalDataAsync(
        string symbol,
        TimeFrame timeframe,
        IReadOnlyList<Candle> candles,
        TimeSpan expiration,
        CancellationToken cancellationToken);

    Task RemoveAsync(
        string symbol,
        CancellationToken cancellationToken);

    Task ClearAsync(
        CancellationToken cancellationToken);
}