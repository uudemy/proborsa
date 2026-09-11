namespace StockPro.Domain.Entities;

public sealed class AppSetting
{
    public Guid Id { get; set; }

    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }
}