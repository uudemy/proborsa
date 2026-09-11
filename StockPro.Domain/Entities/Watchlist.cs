namespace StockPro.Domain.Entities;

public sealed class Watchlist
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<WatchlistItem> Items { get; set; } = new List<WatchlistItem>();
}