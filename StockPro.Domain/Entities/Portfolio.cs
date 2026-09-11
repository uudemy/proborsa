namespace StockPro.Domain.Entities;

public sealed class Portfolio
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal InitialCash { get; set; }

    public decimal CashBalance { get; set; }

    public decimal InvestedValue { get; set; }

    public decimal PortfolioValue { get; set; }

    public decimal UnrealizedProfitLoss { get; set; }

    public decimal RealizedProfitLoss { get; set; }

    public decimal DailyProfitLoss { get; set; }

    public decimal TotalReturn { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<Position> Positions { get; set; } = new List<Position>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public ICollection<Trade> Trades { get; set; } = new List<Trade>();
}