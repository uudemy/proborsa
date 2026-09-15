using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockPro.App.Models;
using StockPro.Application.Interfaces;
using StockPro.Domain.Entities;
using StockPro.Domain.Enums;

namespace StockPro.App.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IMarketDataService _marketDataService;

    private readonly DispatcherTimer _refreshTimer;

    private CancellationTokenSource?
        _refreshCancellationTokenSource;

    private static readonly string[] WatchSymbols =
    [
        "THYAO",
        "ASELS",
        "TUPRS",
        "SISE"
    ];

    private static readonly string[] MovementSymbols =
    [
        "THYAO",
        "ASELS",
        "TUPRS",
        "SISE",
        "EREGL",
        "KCHOL",
        "PETKM",
        "YKBNK",
        "GARAN",
        "AKBNK"
    ];

    public DashboardViewModel(
        IMarketDataService marketDataService)
    {
        _marketDataService =
            marketDataService;

        MarketCards = [];

        TopGainers = [];

        TopLosers = [];

        Watchlist = [];

        ChartPoints = [];

        _refreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };

        _refreshTimer.Tick +=
            RefreshTimer_Tick;

        _ = LoadMarketDataAsync();
    }

    public ObservableCollection<MarketCardViewModel>
        MarketCards { get; }

    public ObservableCollection<StockMovementViewModel>
        TopGainers { get; }

    public ObservableCollection<StockMovementViewModel>
        TopLosers { get; }

    public ObservableCollection<WatchlistItemViewModel>
        Watchlist { get; }

    public ObservableCollection<ChartPoint>
        ChartPoints { get; }

    [ObservableProperty]
    private string _selectedPeriod = "1G";

    [ObservableProperty]
    private string _marketStatus =
        "Veri bekleniyor...";

    [ObservableProperty]
    private string _lastUpdated = "-";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage =
        string.Empty;

    private async void RefreshTimer_Tick(
        object? sender,
        EventArgs e)
    {
        await LoadMarketDataAsync();
    }

    [RelayCommand]
    private async Task SelectPeriodAsync(
        string? period)
    {
        if (string.IsNullOrWhiteSpace(period))
        {
            return;
        }

        if (SelectedPeriod == period)
        {
            return;
        }

        SelectedPeriod = period;

        await LoadChartAsync();
    }

    private async Task LoadMarketDataAsync()
    {
        if (IsLoading)
        {
            return;
        }

        _refreshCancellationTokenSource?.Cancel();

        _refreshCancellationTokenSource?.Dispose();

        _refreshCancellationTokenSource =
            new CancellationTokenSource();

        var cancellationToken =
            _refreshCancellationTokenSource.Token;

        try
        {
            IsLoading = true;

            ErrorMessage =
                string.Empty;

            var marketSymbols =
                new[]
                {
                    "BIST100",
                    "USDTRY",
                    "EURTRY",
                    "GOLD"
                };

            var marketTasks =
                marketSymbols
                    .Select(
                        symbol =>
                            GetMarketQuoteAsync(
                                symbol,
                                cancellationToken))
                    .ToArray();

            var marketQuotes =
                await Task.WhenAll(
                    marketTasks);

            cancellationToken
                .ThrowIfCancellationRequested();

            var bistQuote =
                marketQuotes[0];

            var usdTryQuote =
                marketQuotes[1];

            var eurTryQuote =
                marketQuotes[2];

            var goldQuote =
                marketQuotes[3];

            BuildMarketCards(
                bistQuote,
                usdTryQuote,
                eurTryQuote,
                goldQuote);

            await LoadStockMovementsAsync(
                cancellationToken);

            await LoadWatchlistAsync(
                cancellationToken);

            await LoadChartAsync(
                cancellationToken);

            MarketStatus =
                DetermineMarketStatus();

            LastUpdated =
                DateTime.Now.ToString(
                    "dd.MM.yyyy HH:mm:ss",
                    CultureInfo
                        .GetCultureInfo("tr-TR"));

            if (!_refreshTimer.IsEnabled)
            {
                _refreshTimer.Start();
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            ErrorMessage =
                $"Veri alınamadı: {ex.Message}";

            MarketStatus =
                "Veri bağlantısı başarısız";

            if (!_refreshTimer.IsEnabled)
            {
                _refreshTimer.Start();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task<Quote?> GetMarketQuoteAsync(
        string symbol,
        CancellationToken cancellationToken)
    {
        return await _marketDataService
            .GetQuoteAsync(
                symbol,
                cancellationToken);
    }

    private void BuildMarketCards(
        Quote? bistQuote,
        Quote? usdTryQuote,
        Quote? eurTryQuote,
        Quote? goldQuote)
    {
        MarketCards.Clear();

        if (bistQuote is not null)
        {
            MarketCards.Add(
                CreateMarketCard(
                    "BIST 100",
                    bistQuote));
        }

        if (usdTryQuote is not null)
        {
            MarketCards.Add(
                CreateMarketCard(
                    "USD / TRY",
                    usdTryQuote));
        }

        if (eurTryQuote is not null)
        {
            MarketCards.Add(
                CreateMarketCard(
                    "EUR / TRY",
                    eurTryQuote));
        }

        if (goldQuote is not null)
        {
            var usdTry =
                usdTryQuote?.LastPrice ?? 0;

            var gramGold =
                usdTry > 0
                    ? goldQuote.LastPrice *
                      usdTry /
                      31.1034768m
                    : goldQuote.LastPrice;

            var previousGramGold =
                usdTry > 0
                    ? goldQuote.PreviousClose *
                      usdTry /
                      31.1034768m
                    : goldQuote.PreviousClose;

            var change =
                gramGold -
                previousGramGold;

            var changePercent =
                previousGramGold == 0
                    ? 0
                    : change /
                      previousGramGold *
                      100m;

            MarketCards.Add(
                new MarketCardViewModel(
                    "GRAM ALTIN",
                    FormatPrice(
                        gramGold),
                    FormatChange(
                        change,
                        changePercent),
                    change >= 0));
        }
    }

    private static MarketCardViewModel
        CreateMarketCard(
            string name,
            Quote quote)
    {
        return new MarketCardViewModel(
            name,
            FormatPrice(
                quote.LastPrice),
            FormatChange(
                quote.Change,
                quote.ChangePercent),
            quote.Change >= 0);
    }

    private async Task LoadStockMovementsAsync(
        CancellationToken cancellationToken)
    {
        var quotes =
            await _marketDataService
                .GetQuotesAsync(
                    MovementSymbols,
                    cancellationToken);

        var quoteItems =
            new List<QuoteItem>();

        for (int i = 0;
             i < quotes.Count &&
             i < MovementSymbols.Length;
             i++)
        {
            var quote =
                quotes[i];

            if (quote is null)
            {
                continue;
            }

            quoteItems.Add(
                new QuoteItem(
                    MovementSymbols[i],
                    quote));
        }

        var validQuotes =
            quoteItems
                .Where(
                    x =>
                        x.Quote.PreviousClose > 0)
                .ToList();

        TopGainers.Clear();

        foreach (
            var item in validQuotes
                .OrderByDescending(
                    x =>
                        x.Quote.ChangePercent)
                .Take(4))
        {
            TopGainers.Add(
                new StockMovementViewModel(
                    item.Symbol,
                    FormatPrice(
                        item.Quote.LastPrice),
                    FormatPercent(
                        item.Quote.ChangePercent)));
        }

        TopLosers.Clear();

        foreach (
            var item in validQuotes
                .OrderBy(
                    x =>
                        x.Quote.ChangePercent)
                .Take(4))
        {
            TopLosers.Add(
                new StockMovementViewModel(
                    item.Symbol,
                    FormatPrice(
                        item.Quote.LastPrice),
                    FormatPercent(
                        item.Quote.ChangePercent)));
        }
    }

    private async Task LoadWatchlistAsync(
        CancellationToken cancellationToken)
    {
        var quotes =
            await _marketDataService
                .GetQuotesAsync(
                    WatchSymbols,
                    cancellationToken);

        Watchlist.Clear();

        for (int i = 0;
             i < quotes.Count &&
             i < WatchSymbols.Length;
             i++)
        {
            var quote =
                quotes[i];

            if (quote is null)
            {
                continue;
            }

            Watchlist.Add(
                new WatchlistItemViewModel(
                    WatchSymbols[i],
                    FormatPrice(
                        quote.LastPrice),
                    FormatPercent(
                        quote.ChangePercent),
                    quote.ChangePercent >= 0));
        }
    }

    private async Task LoadChartAsync(
        CancellationToken cancellationToken =
            default)
    {
        var timeframe =
            SelectedPeriod switch
            {
                "1G" =>
                    TimeFrame.OneDay,

                "1H" =>
                    TimeFrame.OneHour,

                "1A" =>
                    TimeFrame.OneDay,

                "1Y" =>
                    TimeFrame.OneWeek,

                _ =>
                    TimeFrame.OneDay
            };

        var candles =
            await _marketDataService
                .GetHistoricalDataAsync(
                    "BIST100",
                    timeframe,
                    cancellationToken);

        var filtered =
            SelectedPeriod switch
            {
                "1G" =>
                    candles
                        .TakeLast(40)
                        .ToList(),

                "1H" =>
                    candles
                        .TakeLast(30)
                        .ToList(),

                "1A" =>
                    candles
                        .TakeLast(31)
                        .ToList(),

                "1Y" =>
                    candles
                        .TakeLast(52)
                        .ToList(),

                _ =>
                    candles.ToList()
            };

        ChartPoints.Clear();

        foreach (var candle in filtered)
        {
            ChartPoints.Add(
                new ChartPoint(
                    candle.Timestamp,
                    candle.Close));
        }
    }

    private static string
        DetermineMarketStatus()
    {
        var now =
            DateTime.Now;

        var day =
            now.DayOfWeek;

        if (day == DayOfWeek.Saturday ||
            day == DayOfWeek.Sunday)
        {
            return "Piyasa Kapalı";
        }

        var open =
            new TimeSpan(
                10,
                0,
                0);

        var close =
            new TimeSpan(
                18,
                0,
                0);

        return now.TimeOfDay >= open &&
               now.TimeOfDay <= close
            ? "Piyasa Açık"
            : "Piyasa Kapalı";
    }

    private static string FormatPrice(
        decimal value)
    {
        return value.ToString(
            "N2",
            CultureInfo
                .GetCultureInfo("tr-TR"));
    }

    private static string FormatPercent(
        decimal value)
    {
        return string.Format(
            CultureInfo
                .GetCultureInfo("tr-TR"),
            "{0:+0.00;-0.00;0.00}%",
            value);
    }

    private static string FormatChange(
        decimal change,
        decimal changePercent)
    {
        var arrow =
            change >= 0
                ? "▲"
                : "▼";

        return string.Format(
            CultureInfo
                .GetCultureInfo("tr-TR"),
            "{0} {1:N2}   {2}",
            arrow,
            Math.Abs(change),
            FormatPercent(
                changePercent));
    }

    private sealed record QuoteItem(
        string Symbol,
        Quote Quote);
}

public sealed record MarketCardViewModel(
    string Name,
    string Value,
    string Change,
    bool IsPositive);

public sealed record StockMovementViewModel(
    string Symbol,
    string Price,
    string Change);

public sealed record WatchlistItemViewModel(
    string Symbol,
    string Price,
    string Change,
    bool IsPositive);