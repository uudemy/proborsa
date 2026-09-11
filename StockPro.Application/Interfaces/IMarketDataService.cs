using StockPro.Domain.Entities;
using StockPro.Domain.Enums;

namespace StockPro.Application.Interfaces;

public interface IMarketDataService
{
    Task<Quote?> GetQuoteAsync(
        string symbol,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Candle>> GetHistoricalDataAsync(
        string symbol,
        TimeFrame timeframe,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Quote>> GetQuotesAsync(
        IEnumerable<string> symbols,
        CancellationToken cancellationToken);
}