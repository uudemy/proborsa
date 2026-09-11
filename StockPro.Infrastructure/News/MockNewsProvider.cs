using StockPro.Domain.Entities;

namespace StockPro.Infrastructure.News;

public sealed class MockNewsProvider
{
    private readonly IReadOnlyList<NewsItem> _news =
    [
        new NewsItem
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            Title = "BIST 100 günün ilk yarısında pozitif seyrediyor",
            Summary = "BIST 100 endeksinde bankacılık ve sanayi hisseleri öne çıkıyor.",
            Content = "Demo haber içeriğidir. Gerçek haber sağlayıcısı bağlandığında bu alan güncellenecektir.",
            Source = "StockPro Demo",
            Author = "StockPro",
            Url = "https://example.com/news/bist-100",
            RelatedSymbol = null,
            PublishedAt = DateTime.UtcNow.AddMinutes(-15),
            RetrievedAt = DateTime.UtcNow,
            IsRead = false
        },
        new NewsItem
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
            Title = "THYAO işlem hacminde dikkat çekiyor",
            Summary = "THYAO hissesi demo piyasa verilerinde yüksek işlem hacmiyle öne çıkıyor.",
            Content = "Demo haber içeriğidir. Gerçek haber sağlayıcısı bağlandığında bu alan güncellenecektir.",
            Source = "StockPro Demo",
            Author = "StockPro",
            Url = "https://example.com/news/thyao",
            RelatedSymbol = "THYAO",
            PublishedAt = DateTime.UtcNow.AddMinutes(-30),
            RetrievedAt = DateTime.UtcNow,
            IsRead = false
        },
        new NewsItem
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
            Title = "Bankacılık endeksi piyasada takip ediliyor",
            Summary = "GARAN ve AKBNK hisseleri bankacılık sektöründeki hareketlilikte öne çıkıyor.",
            Content = "Demo haber içeriğidir. Gerçek haber sağlayıcısı bağlandığında bu alan güncellenecektir.",
            Source = "StockPro Demo",
            Author = "StockPro",
            Url = "https://example.com/news/banking",
            RelatedSymbol = "GARAN",
            PublishedAt = DateTime.UtcNow.AddHours(-1),
            RetrievedAt = DateTime.UtcNow,
            IsRead = false
        },
        new NewsItem
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
            Title = "Sanayi hisselerinde işlem hacmi artıyor",
            Summary = "ASELS, TUPRS ve SISE demo piyasasında yatırımcıların takibinde.",
            Content = "Demo haber içeriğidir. Gerçek haber sağlayıcısı bağlandığında bu alan güncellenecektir.",
            Source = "StockPro Demo",
            Author = "StockPro",
            Url = "https://example.com/news/industry",
            RelatedSymbol = "ASELS",
            PublishedAt = DateTime.UtcNow.AddHours(-2),
            RetrievedAt = DateTime.UtcNow,
            IsRead = false
        }
    ];

    public Task<IReadOnlyList<NewsItem>> GetLatestAsync(
        int count,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (count <= 0)
        {
            return Task.FromResult<IReadOnlyList<NewsItem>>([]);
        }

        IReadOnlyList<NewsItem> result = _news
            .OrderByDescending(news => news.PublishedAt)
            .Take(count)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<NewsItem>> GetBySymbolAsync(
        string symbol,
        int count,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(symbol) || count <= 0)
        {
            return Task.FromResult<IReadOnlyList<NewsItem>>([]);
        }

        var normalizedSymbol = symbol.Trim();

        IReadOnlyList<NewsItem> result = _news
            .Where(news =>
                string.Equals(
                    news.RelatedSymbol,
                    normalizedSymbol,
                    StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(news => news.PublishedAt)
            .Take(count)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<NewsItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var news = _news.FirstOrDefault(item => item.Id == id);

        return Task.FromResult(news);
    }
}