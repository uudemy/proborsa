namespace StockPro.Domain.Entities;

public sealed class Candle
{
    public Guid Id { get; set; }

    public Guid SymbolId { get; set; }

    public DateTime Timestamp { get; set; }

    public decimal Open { get; set; }

    public decimal High { get; set; }

    public decimal Low { get; set; }

    public decimal Close { get; set; }

    public long Volume { get; set; }

    public Symbol? Symbol { get; set; }
}