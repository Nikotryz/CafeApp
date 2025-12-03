using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace CafeApp.WaiterWindows;

public partial class WaiterWindow : Window
{
    public WaiterWindow()
    {
        InitializeComponent();
    }

    private void LogOutBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }
}