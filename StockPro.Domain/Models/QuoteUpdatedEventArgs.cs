using StockPro.Domain.Entities;

namespace StockPro.Domain.Models;

public sealed class QuoteUpdatedEventArgs : EventArgs
{
    public QuoteUpdatedEventArgs(Quote quote)
    {
        Quote = quote ?? throw new ArgumentNullException(nameof(quote));
    }

    public Quote Quote { get; }
}