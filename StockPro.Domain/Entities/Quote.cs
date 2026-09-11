namespace StockPro.Domain.Entities;

public sealed class Quote
{
    public Guid Id { get; set; }

    public Guid SymbolId { get; set; }

    public decimal LastPrice { get; set; }

    public decimal OpenPrice { get; set; }

    public decimal HighPrice { get; set; }

    public decimal LowPrice { get; set; }

    public decimal PreviousClose { get; set; }

    public decimal Change { get; set; }

    public decimal ChangePercent { get; set; }

    public long Volume { get; set; }

    public DateTime Timestamp { get; set; }

    public Symbol? Symbol { get; set; }
}