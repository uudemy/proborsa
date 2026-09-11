namespace StockPro.Domain.Entities;

public sealed class Symbol
{
    public Guid Id { get; set; }

    public string Ticker { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Exchange { get; set; } = string.Empty;

    public string Currency { get; set; } = "TRY";

    public string? Sector { get; set; }

    public bool IsActive { get; set; } = true;
}