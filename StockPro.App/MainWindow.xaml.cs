using System.Windows;
using StockPro.App.ViewModels;

namespace StockPro.App;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}