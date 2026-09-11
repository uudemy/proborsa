namespace StockPro.App.Models;

public sealed record NavigationItem(
    string Key,
    string Title,
    string Icon,
    string Group);