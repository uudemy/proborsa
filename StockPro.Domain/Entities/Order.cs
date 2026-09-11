using StockPro.Domain.Enums;

namespace StockPro.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; set; }

    public Guid PortfolioId { get; set; }

    public Guid SymbolId { get; set; }

    public OrderSide Side { get; set; }

    public OrderType Type { get; set; }

    public OrderStatus Status { get; set; }

    public decimal Quantity { get; set; }

    public decimal? LimitPrice { get; set; }

    public decimal? StopPrice { get; set; }

    public decimal? StopLossPrice { get; set; }

    public decimal? TakeProfitPrice { get; set; }

    public decimal FilledQuantity { get; set; }

    public decimal? AverageFillPrice { get; set; }

    public decimal EstimatedCost { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public DateTime? FilledAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public Portfolio? Portfolio { get; set; }

    public Symbol? Symbol { get; set; }

    public ICollection<Trade> Trades { get; set; } = new List<Trade>();
}