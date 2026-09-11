using StockPro.Domain.Enums;

namespace StockPro.Domain.Entities;

public sealed class Alert
{
    public Guid Id { get; set; }

    public Guid SymbolId { get; set; }

    public AlertType Type { get; set; }

    public decimal Threshold { get; set; }

    public decimal? SecondaryThreshold { get; set; }

    public bool IsEnabled { get; set; } = true;

    public bool IsTriggered { get; set; }

    public bool EnableDesktopNotification { get; set; } = true;

    public bool EnableSound { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? TriggeredAt { get; set; }

    public Symbol? Symbol { get; set; }
}