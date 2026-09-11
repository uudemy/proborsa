using StockPro.Domain.Entities;
using StockPro.Domain.Enums;
using StockPro.Domain.Interfaces;

namespace StockPro.Infrastructure.MarketData;

public sealed class MockMarketDataProvider : IMarketDataProvider
{
    private static readonly string[] SupportedSymbols =
    [
        "THYAO",
        "ASELS",
        "TUPRS",
        "BIMAS",
        "GARAN",
        "AKBNK",
        "KCHOL",
        "SAHOL",
        "SISE",
        "EREGL"
    ];

    private readonly Random _random = new();

    public Task<Quote?> GetQuoteAsync(
        string symbol,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedSymbol = NormalizeSymbol(symbol);

        if (!SupportedSymbols.Contains(
                normalizedSymbol,
                StringComparer.OrdinalIgnoreCase))
        {
            return Task.FromResult<Quote?>(null);
        }

        var now = DateTime.UtcNow;
        var previousClose = GetBasePrice(normalizedSymbol);

        var changePercent = (decimal)(_random.NextDouble() * 6.0 - 3.0);
        var lastPrice = previousClose * (1m + changePercent / 100m);

        lastPrice = Math.Round(
            Math.Max(0.01m, lastPrice),
            2);

        var change = Math.Round(
            lastPrice - previousClose,
            2);

        var highPrice = Math.Round(
            Math.Max(lastPrice, previousClose) *
            (1m + (decimal)_random.NextDouble() * 0.015m),
            2);

        var lowPrice = Math.Round(
            Math.Min(lastPrice, previousClose) *
            (1m - (decimal)_random.NextDouble() * 0.015m),
            2);

        var quote = new Quote
        {
            Id = Guid.NewGuid(),
            SymbolId = Guid.NewGuid(),
            LastPrice = lastPrice,
            OpenPrice = previousClose,
            HighPrice = Math.Max(highPrice, lastPrice),
            LowPrice = Math.Min(lowPrice, lastPrice),
            PreviousClose = previousClose,
            Change = change,
            ChangePercent = Math.Round(
                change / previousClose * 100m,
                2),
            Volume = _random.NextInt64(
                500_000,
                25_000_000),
            Timestamp = now
        };

        return Task.FromResult<Quote?>(quote);
    }

    public Task<IReadOnlyList<Candle>> GetHistoricalDataAsync(
        string symbol,
        TimeFrame timeframe,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedSymbol = NormalizeSymbol(symbol);

        if (!SupportedSymbols.Contains(
                normalizedSymbol,
                StringComparer.OrdinalIgnoreCase))
        {
            return Task.FromResult<IReadOnlyList<Candle>>([]);
        }

        var count = GetCandleCount(timeframe);
        var interval = GetInterval(timeframe);
        var basePrice = GetBasePrice(normalizedSymbol);

        var candles = new List<Candle>(count);
        var currentPrice = basePrice;
        var endTime = DateTime.UtcNow;

        for (var index = count - 1; index >= 0; index--)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var timestamp = endTime - TimeSpan.FromTicks(
                interval.Ticks * index);

            var open = currentPrice;

            var movementPercent =
                (decimal)(_random.NextDouble() * 4.0 - 2.0);

            var close = Math.Round(
                Math.Max(
                    0.01m,
                    open * (1m + movementPercent / 100m)),
                2);

            var high = Math.Round(
                Math.Max(open, close) *
                (1m + (decimal)_random.NextDouble() * 0.01m),
                2);

            var low = Math.Round(
                Math.Min(open, close) *
                (1m - (decimal)_random.NextDouble() * 0.01m),
                2);

            candles.Add(new Candle
            {
                Id = Guid.NewGuid(),
                SymbolId = Guid.NewGuid(),
                Timestamp = timestamp,
                Open = open,
                High = Math.Max(high, Math.Max(open, close)),
                Low = Math.Min(low, Math.Min(open, close)),
                Close = close,
                Volume = _random.NextInt64(
                    50_000,
                    5_000_000)
            });

            currentPrice = close;
        }

        return Task.FromResult<IReadOnlyList<Candle>>(candles);
    }

    private decimal GetBasePrice(string symbol)
    {
        return symbol.ToUpperInvariant() switch
        {
            "THYAO" => 320.00m,
            "ASELS" => 175.00m,
            "TUPRS" => 185.00m,
            "BIMAS" => 560.00m,
            "GARAN" => 145.00m,
            "AKBNK" => 78.00m,
            "KCHOL" => 210.00m,
            "SAHOL" => 105.00m,
            "SISE" => 48.00m,
            "EREGL" => 62.00m,
            _ => 100.00m
        };
    }

    private static int GetCandleCount(TimeFrame timeframe)
    {
        return timeframe switch
        {
            TimeFrame.OneMinute => 240,
            TimeFrame.FiveMinutes => 240,
            TimeFrame.FifteenMinutes => 240,
            TimeFrame.ThirtyMinutes => 240,
            TimeFrame.OneHour => 240,
            TimeFrame.FourHours => 180,
            TimeFrame.OneDay => 180,
            TimeFrame.OneWeek => 104,
            TimeFrame.OneMonth => 60,
            _ => 240
        };
    }

    private static TimeSpan GetInterval(TimeFrame timeframe)
    {
        return timeframe switch
        {
            TimeFrame.OneMinute => TimeSpan.FromMinutes(1),
            TimeFrame.FiveMinutes => TimeSpan.FromMinutes(5),
            TimeFrame.FifteenMinutes => TimeSpan.FromMinutes(15),
            TimeFrame.ThirtyMinutes => TimeSpan.FromMinutes(30),
            TimeFrame.OneHour => TimeSpan.FromHours(1),
            TimeFrame.FourHours => TimeSpan.FromHours(4),
            TimeFrame.OneDay => TimeSpan.FromDays(1),
            TimeFrame.OneWeek => TimeSpan.FromDays(7),
            TimeFrame.OneMonth => TimeSpan.FromDays(30),
            _ => TimeSpan.FromMinutes(1)
        };
    }

    private static string NormalizeSymbol(string symbol)
    {
        return symbol.Trim().ToUpperInvariant();
    }
}