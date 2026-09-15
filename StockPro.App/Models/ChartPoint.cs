namespace StockPro.App.Models;

public sealed record ChartPoint(
    DateTime Time,
    decimal Value);