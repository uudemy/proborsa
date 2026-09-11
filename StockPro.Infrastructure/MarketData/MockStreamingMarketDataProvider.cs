using StockPro.Domain.Entities;
using StockPro.Domain.Interfaces;
using StockPro.Domain.Models;

namespace StockPro.Infrastructure.MarketData;

public sealed class MockStreamingMarketDataProvider :
    IStreamingMarketDataProvider,
    IAsyncDisposable
{
    private readonly object _syncRoot = new();

    private readonly Dictionary<string, Quote> _quotes =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly HashSet<string> _subscribedSymbols =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Random _random = new();

    private CancellationTokenSource? _streamCancellationSource;

    private Task? _streamTask;

    public event EventHandler<QuoteUpdatedEventArgs>? QuoteUpdated;

    public Task SubscribeAsync(
        IEnumerable<string> symbols,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(symbols);

        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            foreach (var symbol in symbols)
            {
                if (!string.IsNullOrWhiteSpace(symbol))
                {
                    _subscribedSymbols.Add(
                        symbol.Trim().ToUpperInvariant());
                }
            }

            EnsureStreamingStarted();
        }

        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync(
        IEnumerable<string> symbols,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(symbols);

        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            foreach (var symbol in symbols)
            {
                if (!string.IsNullOrWhiteSpace(symbol))
                {
                    _subscribedSymbols.Remove(
                        symbol.Trim().ToUpperInvariant());
                }
            }

            if (_subscribedSymbols.Count == 0)
            {
                StopStreaming();
            }
        }

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        CancellationTokenSource? cancellationSource;

        lock (_syncRoot)
        {
            cancellationSource = _streamCancellationSource;
            _streamCancellationSource = null;
            _streamTask = null;
            _subscribedSymbols.Clear();
        }

        if (cancellationSource is not null)
        {
            await cancellationSource.CancelAsync();

            cancellationSource.Dispose();
        }
    }

    private void EnsureStreamingStarted()
    {
        if (_streamTask is { IsCompleted: false })
        {
            return;
        }

        _streamCancellationSource?.Dispose();

        _streamCancellationSource = new CancellationTokenSource();

        var cancellationToken =
            _streamCancellationSource.Token;

        _streamTask = Task.Run(
            () => StreamAsync(cancellationToken),
            cancellationToken);
    }

    private void StopStreaming()
    {
        if (_streamCancellationSource is null)
        {
            return;
        }

        _streamCancellationSource.Cancel();

        _streamCancellationSource.Dispose();

        _streamCancellationSource = null;
        _streamTask = null;
    }

    private async Task StreamAsync(
        CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(
            TimeSpan.FromSeconds(1));

        try
        {
            while (await timer.WaitForNextTickAsync(
                       cancellationToken))
            {
                string[] symbols;

                lock (_syncRoot)
                {
                    symbols = _subscribedSymbols.ToArray();
                }

                foreach (var symbol in symbols)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var quote = CreateNextQuote(symbol);

                    QuoteUpdated?.Invoke(
                        this,
                        new QuoteUpdatedEventArgs(quote));
                }
            }
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    private Quote CreateNextQuote(string symbol)
    {
        lock (_syncRoot)
        {
            if (!_quotes.TryGetValue(symbol, out var previous))
            {
                var basePrice = GetBasePrice(symbol);

                previous = new Quote
                {
                    Id = Guid.NewGuid(),
                    SymbolId = Guid.NewGuid(),
                    LastPrice = basePrice,
                    OpenPrice = basePrice,
                    HighPrice = basePrice,
                    LowPrice = basePrice,
                    PreviousClose = basePrice,
                    Timestamp = DateTime.UtcNow
                };
            }

            var movementPercent =
                (decimal)(_random.NextDouble() * 0.8 - 0.4);

            var lastPrice = Math.Round(
                Math.Max(
                    0.01m,
                    previous.LastPrice *
                    (1m + movementPercent / 100m)),
                2);

            var high = Math.Max(
                previous.HighPrice,
                lastPrice);

            var low = Math.Min(
                previous.LowPrice,
                lastPrice);

            var change = Math.Round(
                lastPrice - previous.PreviousClose,
                2);

            var quote = new Quote
            {
                Id = previous.Id,
                SymbolId = previous.SymbolId,
                LastPrice = lastPrice,
                OpenPrice = previous.OpenPrice,
                HighPrice = high,
                LowPrice = low,
                PreviousClose = previous.PreviousClose,
                Change = change,
                ChangePercent = previous.PreviousClose == 0
                    ? 0
                    : Math.Round(
                        change /
                        previous.PreviousClose *
                        100m,
                        2),
                Volume = previous.Volume +
                         _random.NextInt64(
                             1_000,
                             100_000),
                Timestamp = DateTime.UtcNow
            };

            _quotes[symbol] = quote;

            return quote;
        }
    }

    private static decimal GetBasePrice(string symbol)
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
}