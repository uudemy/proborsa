namespace StockPro.Domain.Enums;

/// <summary>
/// Represents the type of alert to be triggered.
/// </summary>
public enum AlertType
{
    PriceAbove,
    PriceBelow,
    PercentageChange,
    Volume,
    Rsi
}