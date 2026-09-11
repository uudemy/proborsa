using StockPro.Application.Interfaces;
using StockPro.Domain.Entities;
using StockPro.Domain.Enums;
using StockPro.Domain.Interfaces;
using StockPro.Infrastructure.Repositories;

namespace StockPro.Infrastructure.Services;

public sealed class MarketDataService : IMarketDataService
{
    private static readonly TimeSpan QuoteCacheExpiration =
        TimeSpan.FromSeconds(5);

    private static readonly TimeSpan HistoricalCacheExpiration =
        TimeSpan.FromMinutes(5);

    private readonly IMarketDataProvider _provider;

    private readonly IMarketDataCache _cache;

    public MarketDataService(
        IMarketDataProvider provider,
        IMarketDataCache cache)
    {
        _provider = provider;
        _cache = cache;
    }

    public async Task<Quote?> GetQuoteAsync(
        string symbol,
        CancellationToken cancellationToken)
    {
        ValidateSymbol(symbol);

        var normalizedSymbol = NormalizeSymbol(symbol);

        var cachedQuote = await _cache.GetQuoteAsync(
            normalizedSymbol,
            cancellationToken);

        if (cachedQuote is not null)
        {
            return cachedQuote;
        }

        var quote = await _provider.GetQuoteAsync(
            normalizedSymbol,
            cancellationToken);

        if (quote is not null)
        {
            await _cache.SetQuoteAsync(
                normalizedSymbol,
                quote,
                QuoteCacheExpiration,
                cancellationToken);
        }

        return quote;
    }

    public async Task<IReadOnlyList<Candle>> GetHistoricalDataAsync(
        string symbol,
        TimeFrame timeframe,
        CancellationToken cancellationToken)
    {
        ValidateSymbol(symbol);

        var normalizedSymbol = NormalizeSymbol(symbol);

        var cachedCandles =
            await _cache.GetHistoricalDataAsync(
                normalizedSymbol,
                timeframe,
                cancellationToken);

        if (cachedCandles is not null)
        {
            return cachedCandles;
        }

        var candles =
            await _provider.GetHistoricalDataAsync(
                normalizedSymbol,
                timeframe,
                cancellationToken);

        if (candles.Count > 0)
        {
            await _cache.SetHistoricalDataAsync(
                normalizedSymbol,
                timeframe,
                candles,
                HistoricalCacheExpiration,
                cancellationToken);
        }

        return candles;
    }

    public async Task<IReadOnlyList<Quote>> GetQuotesAsync(
        IEnumerable<string> symbols,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(symbols);

        var normalizedSymbols = symbols
            .Where(symbol => !string.IsNullOrWhiteSpace(symbol))
            .Select(NormalizeSymbol)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedSymbols.Count == 0)
        {
            return [];
        }

        var quotes = new List<Quote>(normalizedSymbols.Count);

        foreach (var symbol in normalizedSymbols)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var quote = await GetQuoteAsync(
                symbol,
                cancellationToken);

            if (quote is not null)
            {
                quotes.Add(quote);
            }
        }

        return quotes;
    }

    private static void ValidateSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException(
                "Symbol cannot be empty.",
                nameof(symbol));
        }
    }

    private static string NormalizeSymbol(string symbol)
    {
        return symbol.Trim().ToUpperInvariant();
    }
}