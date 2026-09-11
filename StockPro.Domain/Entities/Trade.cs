using StockPro.Domain.Enums;

namespace StockPro.Domain.Entities;

public sealed class Trade
{
    public Guid Id { get; set; }

    public Guid PortfolioId { get; set; }

    public Guid OrderId { get; set; }

    public Guid SymbolId { get; set; }

    public OrderSide Side { get; set; }

    public decimal Quantity { get; set; }

    public decimal ExecutionPrice { get; set; }

    public decimal Commission { get; set; }

    public decimal TotalValue { get; set; }

    public DateTime ExecutedAt { get; set; }

    public Portfolio? Portfolio { get; set; }

    public Order? Order { get; set; }

    public Symbol? Symbol { get; set; }
}