using StockPro.Domain.Entities;
using StockPro.Domain.Enums;
using StockPro.Infrastructure.Repositories;

namespace StockPro.Infrastructure.Caching;

public sealed class InMemoryMarketDataCache : IMarketDataCache
{
    private sealed record CacheEntry<T>(T Value, DateTimeOffset ExpiresAt);

    private readonly object _syncRoot = new();

    private readonly Dictionary<string, CacheEntry<Quote>> _quotes =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, CacheEntry<IReadOnlyList<Candle>>> _historicalData =
        new(StringComparer.OrdinalIgnoreCase);

    public Task<Quote?> GetQuoteAsync(
        string symbol,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(symbol))
        {
            return Task.FromResult<Quote?>(null);
        }

        var key = symbol.Trim();

        lock (_syncRoot)
        {
            if (!_quotes.TryGetValue(key, out var entry))
            {
                return Task.FromResult<Quote?>(null);
            }

            if (entry.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                _quotes.Remove(key);
                return Task.FromResult<Quote?>(null);
            }

            return Task.FromResult<Quote?>(entry.Value);
        }
    }

    public Task SetQuoteAsync(
        string symbol,
        Quote quote,
        TimeSpan expiration,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(quote);

        cancellationToken.ThrowIfCancellationRequested();

        ValidateExpiration(expiration);

        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException(
                "Symbol cannot be empty.",
                nameof(symbol));
        }

        var key = symbol.Trim();

        lock (_syncRoot)
        {
            _quotes[key] = new CacheEntry<Quote>(
                quote,
                DateTimeOffset.UtcNow.Add(expiration));
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Candle>?> GetHistoricalDataAsync(
        string symbol,
        TimeFrame timeframe,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(symbol))
        {
            return Task.FromResult<IReadOnlyList<Candle>?>(null);
        }

        var key = CreateHistoricalDataKey(symbol, timeframe);

        lock (_syncRoot)
        {
            if (!_historicalData.TryGetValue(key, out var entry))
            {
                return Task.FromResult<IReadOnlyList<Candle>?>(null);
            }

            if (entry.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                _historicalData.Remove(key);
                return Task.FromResult<IReadOnlyList<Candle>?>(null);
            }

            return Task.FromResult<IReadOnlyList<Candle>?>(entry.Value);
        }
    }

    public Task SetHistoricalDataAsync(
        string symbol,
        TimeFrame timeframe,
        IReadOnlyList<Candle> candles,
        TimeSpan expiration,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(candles);

        cancellationToken.ThrowIfCancellationRequested();

        ValidateExpiration(expiration);

        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException(
                "Symbol cannot be empty.",
                nameof(symbol));
        }

        var key = CreateHistoricalDataKey(symbol, timeframe);

        var snapshot = candles.ToList().AsReadOnly();

        lock (_syncRoot)
        {
            _historicalData[key] = new CacheEntry<IReadOnlyList<Candle>>(
                snapshot,
                DateTimeOffset.UtcNow.Add(expiration));
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(
        string symbol,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(symbol))
        {
            return Task.CompletedTask;
        }

        var normalizedSymbol = symbol.Trim();

        lock (_syncRoot)
        {
            _quotes.Remove(normalizedSymbol);

            var historicalKeys = _historicalData.Keys
                .Where(key => key.StartsWith(
                    $"{normalizedSymbol}:",
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var key in historicalKeys)
            {
                _historicalData.Remove(key);
            }
        }

        return Task.CompletedTask;
    }

    public Task ClearAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            _quotes.Clear();
            _historicalData.Clear();
        }

        return Task.CompletedTask;
    }

    private static string CreateHistoricalDataKey(
        string symbol,
        TimeFrame timeframe)
    {
        return $"{symbol.Trim()}:{timeframe}";
    }

    private static void ValidateExpiration(TimeSpan expiration)
    {
        if (expiration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expiration),
                "Cache expiration must be greater than zero.");
        }
    }
}