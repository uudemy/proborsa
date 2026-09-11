using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockPro.App.Models;

namespace StockPro.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel()
    {
        NavigationItems =
        [
            new NavigationItem("Dashboard", "Dashboard", "▦", "WORKSPACE"),
            new NavigationItem("Markets", "Markets", "◈", "WORKSPACE"),
            new NavigationItem("Watchlist", "Watchlist", "☆", "WORKSPACE"),
            new NavigationItem("Chart", "Chart", "⌁", "WORKSPACE"),
            new NavigationItem("Scanner", "Scanner", "⌕", "WORKSPACE"),

            new NavigationItem("Portfolio", "Portfolio", "▤", "TRADING"),
            new NavigationItem("Orders", "Orders", "↕", "TRADING"),
            new NavigationItem("Trades", "Trades", "≡", "TRADING"),
            new NavigationItem("Alerts", "Alerts", "⚑", "TRADING"),

            new NavigationItem("News", "News", "▰", "INFORMATION"),
            new NavigationItem("Settings", "Settings", "⚙", "INFORMATION")
        ];

        SelectedNavigationItem = NavigationItems[0];
    }

    public ObservableCollection<NavigationItem> NavigationItems { get; }

    [ObservableProperty]
    private string _title = "StockPro";

    [ObservableProperty]
    private string _statusMessage = "Demo piyasa verileri hazır.";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private NavigationItem? _selectedNavigationItem;

    partial void OnSelectedNavigationItemChanged(
        NavigationItem? value)
    {
        if (value is null)
        {
            return;
        }

        StatusMessage = $"{value.Title} ekranı hazır.";
    }

    [RelayCommand]
    private void Navigate(NavigationItem? item)
    {
        if (item is null)
        {
            return;
        }

        SelectedNavigationItem = item;
    }
}