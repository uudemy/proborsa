namespace StockPro.Domain.Entities;

public sealed class NewsItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string? Content { get; set; }

    public string Source { get; set; } = string.Empty;

    public string? Author { get; set; }

    public string Url { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public string? RelatedSymbol { get; set; }

    public DateTime PublishedAt { get; set; }

    public DateTime RetrievedAt { get; set; }

    public bool IsRead { get; set; }
}