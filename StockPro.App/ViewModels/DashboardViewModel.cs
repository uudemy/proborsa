using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        ChartPoints = CreateChartPoints("1G");
    }

    public ObservableCollection<MarketCardViewModel> MarketCards { get; }

    public ObservableCollection<StockMovementViewModel> TopGainers { get; }

    public ObservableCollection<StockMovementViewModel> TopLosers { get; }

    public ObservableCollection<ChartPoint> ChartPoints { get; }

    [ObservableProperty]
    private string _selectedPeriod = "1G";

    public string MarketStatus { get; } = "Piyasa Açık";

    [RelayCommand]
    private void SelectPeriod(string? period)
    {
        if (string.IsNullOrWhiteSpace(period))
            return;

        if (SelectedPeriod == period)
            return;

        SelectedPeriod = period;

        var newPoints = CreateChartPoints(period);

        ChartPoints.Clear();

        foreach (var point in newPoints)
        {
            ChartPoints.Add(point);
        }
    }

    private static ObservableCollection<ChartPoint> CreateChartPoints(
        string period)
    {
        return period switch
        {
            "1G" => CreateDayPoints(),
            "1H" => CreateWeekPoints(),
            "1A" => CreateMonthPoints(),
            "1Y" => CreateYearPoints(),
            _ => CreateDayPoints()
        };
    }

    private static ObservableCollection<ChartPoint> CreateDayPoints()
    {
        var points = new ObservableCollection<ChartPoint>();

        decimal[] values =
        [
            10720,
            10765,
            10740,
            10820,
            10805,
            10855,
            10830,
            10890,
            10865,
            10920,
            10895,
            10842
        ];

        for (int i = 0; i < values.Length; i++)
        {
            points.Add(
                new ChartPoint(
                    DateTime.Today.AddMinutes(570 + i * 30),
                    values[i]));
        }

        return points;
    }

    private static ObservableCollection<ChartPoint> CreateWeekPoints()
    {
        var points = new ObservableCollection<ChartPoint>();

        decimal[] values =
        [
            10620,
            10685,
            10710,
            10655,
            10740,
            10810,
            10785,
            10842
        ];

        for (int i = 0; i < values.Length; i++)
        {
            points.Add(
                new ChartPoint(
                    DateTime.Today.AddDays(-7 + i),
                    values[i]));
        }

        return points;
    }

    private static ObservableCollection<ChartPoint> CreateMonthPoints()
    {
        var points = new ObservableCollection<ChartPoint>();

        decimal[] values =
        [
            10120,
            10240,
            10185,
            10310,
            10420,
            10365,
            10510,
            10480,
            10620,
            10585,
            10710,
            10842
        ];

        for (int i = 0; i < values.Length; i++)
        {
            points.Add(
                new ChartPoint(
                    DateTime.Today.AddDays(-30 + i * 3),
                    values[i]));
        }

        return points;
    }

    private static ObservableCollection<ChartPoint> CreateYearPoints()
    {
        var points = new ObservableCollection<ChartPoint>();

        decimal[] values =
        [
            8420,
            8650,
            8910,
            8760,
            9180,
            9450,
            9320,
            9780,
            10020,
            10280,
            10540,
            10842
        ];

        for (int i = 0; i < values.Length; i++)
        {
            points.Add(
                new ChartPoint(
                    DateTime.Today.AddMonths(-11 + i),
                    values[i]));
        }

        return points;
    }
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