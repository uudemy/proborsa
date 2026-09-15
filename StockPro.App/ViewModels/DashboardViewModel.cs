using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using StockPro.App.Models;

namespace StockPro.App.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    public DashboardViewModel()
    {
        MarketCards =
        [
            new MarketCardViewModel(
                "BIST 100",
                "10.842,31",
                "▲ 124,56   +1,16%",
                true),

            new MarketCardViewModel(
                "USD / TRY",
                "41,2875",
                "▲ 0,0825   +0,20%",
                true),

            new MarketCardViewModel(
                "EUR / TRY",
                "48,6120",
                "▼ 0,1540   -0,32%",
                false),

            new MarketCardViewModel(
                "GRAM ALTIN",
                "4.386,20",
                "▲ 18,40   +0,42%",
                true)
        ];

        TopGainers =
        [
            new StockMovementViewModel("ASELS", "185,40", "+6,42%"),
            new StockMovementViewModel("THYAO", "318,75", "+5,18%"),
            new StockMovementViewModel("TUPRS", "164,20", "+4,73%"),
            new StockMovementViewModel("EREGL", "72,15", "+3,91%")
        ];

        TopLosers =
        [
            new StockMovementViewModel("SISE", "48,32", "-4,12%"),
            new StockMovementViewModel("KCHOL", "182,50", "-3,64%"),
            new StockMovementViewModel("PETKM", "22,84", "-3,27%"),
            new StockMovementViewModel("YKBNK", "34,16", "-2,85%")
        ];

        ChartPoints =
        [
            new ChartPoint(DateTime.Today.AddHours(9.5), 10720),
            new ChartPoint(DateTime.Today.AddHours(10), 10765),
            new ChartPoint(DateTime.Today.AddHours(10.5), 10740),
            new ChartPoint(DateTime.Today.AddHours(11), 10820),
            new ChartPoint(DateTime.Today.AddHours(11.5), 10805),
            new ChartPoint(DateTime.Today.AddHours(12), 10855),
            new ChartPoint(DateTime.Today.AddHours(12.5), 10830),
            new ChartPoint(DateTime.Today.AddHours(13), 10890),
            new ChartPoint(DateTime.Today.AddHours(13.5), 10865),
            new ChartPoint(DateTime.Today.AddHours(14), 10920),
            new ChartPoint(DateTime.Today.AddHours(14.5), 10895),
            new ChartPoint(DateTime.Today.AddHours(15), 10842)
        ];
    }

    public ObservableCollection<MarketCardViewModel> MarketCards { get; }

    public ObservableCollection<StockMovementViewModel> TopGainers { get; }

    public ObservableCollection<StockMovementViewModel> TopLosers { get; }

    public ObservableCollection<ChartPoint> ChartPoints { get; }

    public string SelectedPeriod { get; set; } = "1G";

    public string MarketStatus { get; } = "Piyasa Açık";
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