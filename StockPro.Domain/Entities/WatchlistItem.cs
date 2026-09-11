namespace StockPro.Domain.Entities;

public sealed class WatchlistItem
{
    public Guid Id { get; set; }

    public Guid WatchlistId { get; set; }

    public Guid SymbolId { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime AddedAt { get; set; }

    public Watchlist? Watchlist { get; set; }

    public Symbol? Symbol { get; set; }
}