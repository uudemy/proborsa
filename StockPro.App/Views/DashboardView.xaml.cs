using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using StockPro.App.Models;
using StockPro.App.ViewModels;
using StockPro.Application.Interfaces;

namespace StockPro.App.Views;

public partial class DashboardView : UserControl
{
    private readonly DashboardViewModel _viewModel;

    public DashboardView(
        IMarketDataService marketDataService)
    {
        InitializeComponent();

        _viewModel =
            new DashboardViewModel(
                marketDataService);

        DataContext = _viewModel;

        _viewModel.ChartPoints.CollectionChanged +=
            ChartPoints_CollectionChanged;

        Loaded += DashboardView_Loaded;
        SizeChanged += DashboardView_SizeChanged;
    }

    private void ChartPoints_CollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        Dispatcher.BeginInvoke(DrawChart);
    }

    private void DashboardView_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        DrawChart();
    }

    private void DashboardView_SizeChanged(
        object sender,
        SizeChangedEventArgs e)
    {
        DrawChart();
    }

    private void DrawChart()
    {
        if (BistChartCanvas is null)
            return;

        var points =
            _viewModel.ChartPoints.ToList();

        if (points.Count < 2)
            return;

        BistChartCanvas.Children.Clear();

        double width =
            BistChartCanvas.ActualWidth;

        double height =
            BistChartCanvas.ActualHeight;

        if (width <= 0 || height <= 0)
            return;

        const double leftPadding = 12;
        const double rightPadding = 12;
        const double topPadding = 16;
        const double bottomPadding = 24;

        double chartWidth =
            width -
            leftPadding -
            rightPadding;

        double chartHeight =
            height -
            topPadding -
            bottomPadding;

        if (chartWidth <= 0 ||
            chartHeight <= 0)
        {
            return;
        }

        decimal minValue =
            points.Min(x => x.Value);

        decimal maxValue =
            points.Max(x => x.Value);

        decimal range =
            maxValue - minValue;

        if (range == 0)
            range = 1;

        for (int i = 0; i <= 4; i++)
        {
            double y =
                topPadding +
                chartHeight * i / 4;

            var gridLine =
                new Line
                {
                    X1 = leftPadding,
                    Y1 = y,
                    X2 = width - rightPadding,
                    Y2 = y,
                    Stroke =
                        new SolidColorBrush(
                            Color.FromArgb(
                                45,
                                139,
                                152,
                                168)),
                    StrokeThickness = 1
                };

            BistChartCanvas.Children.Add(
                gridLine);
        }

        var polyline =
            new Polyline
            {
                Stroke =
                    new SolidColorBrush(
                        (Color)ColorConverter
                            .ConvertFromString(
                                "#3B82F6")),
                StrokeThickness = 2.5,
                StrokeLineJoin =
                    PenLineJoin.Round
            };

        for (int i = 0;
             i < points.Count;
             i++)
        {
            double x =
                leftPadding +
                chartWidth *
                i /
                (points.Count - 1);

            double normalized =
                (double)(
                    (points[i].Value -
                     minValue) /
                    range);

            double y =
                topPadding +
                chartHeight -
                normalized *
                chartHeight;

            polyline.Points.Add(
                new Point(x, y));
        }

        BistChartCanvas.Children.Add(
            polyline);

        var lastPoint =
            polyline.Points[^1];

        var marker =
            new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill =
                    new SolidColorBrush(
                        (Color)ColorConverter
                            .ConvertFromString(
                                "#3B82F6"))
            };

        Canvas.SetLeft(
            marker,
            lastPoint.X - 4);

        Canvas.SetTop(
            marker,
            lastPoint.Y - 4);

        BistChartCanvas.Children.Add(
            marker);

        var lastValue =
            points[^1].Value;

        var valueText =
            new TextBlock
            {
                Text =
                    lastValue.ToString(
                        "N2"),
                Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter
                            .ConvertFromString(
                                "#F1F5F9")),
                FontSize = 11,
                FontWeight =
                    FontWeights.SemiBold
            };

        Canvas.SetLeft(
            valueText,
            Math.Max(
                0,
                lastPoint.X - 35));

        Canvas.SetTop(
            valueText,
            Math.Max(
                0,
                lastPoint.Y - 24));

        BistChartCanvas.Children.Add(
            valueText);
    }
}