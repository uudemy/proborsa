using StockPro.Domain.Entities;
using StockPro.Domain.Enums;

namespace StockPro.Domain.Interfaces;

public interface IMarketDataProvider
{
    Task<Quote?> GetQuoteAsync(
        string symbol,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Candle>> GetHistoricalDataAsync(
        string symbol,
        TimeFrame timeframe,
        CancellationToken cancellationToken);
}