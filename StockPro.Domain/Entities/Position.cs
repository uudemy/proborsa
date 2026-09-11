namespace StockPro.Domain.Entities;

public sealed class Position
{
    public Guid Id { get; set; }

    public Guid PortfolioId { get; set; }

    public Guid SymbolId { get; set; }

    public decimal Quantity { get; set; }

    public decimal AverageEntryPrice { get; set; }

    public decimal CurrentPrice { get; set; }

    public decimal MarketValue { get; set; }

    public decimal UnrealizedProfitLoss { get; set; }

    public decimal UnrealizedProfitLossPercent { get; set; }

    public DateTime OpenedAt { get; set; }

    public Portfolio? Portfolio { get; set; }

    public Symbol? Symbol { get; set; }
}