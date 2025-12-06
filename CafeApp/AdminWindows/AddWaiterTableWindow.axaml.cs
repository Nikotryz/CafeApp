using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CafeApp.Models;
using CafeApp.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.AdminWindows;

public partial class AddWaiterTableWindow : Window
{
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    private readonly ComboBox _waiterComboBox;
    private readonly ComboBox _tableComboBox;

    public List<User> Waiters { get; set; } = [];
    public List<Table> Tables { get; set; } = [];
    
    public AddWaiterTableWindow()
    {
        InitializeComponent();
        
        _waiterComboBox = this.FindControl<ComboBox>("WaiterComboBox")!;
        _tableComboBox = this.FindControl<ComboBox>("TableComboBox")!;

        LoadData();
    }

    private void LoadData()
    {
        _waiterComboBox.ItemsSource = _db.Users
            .Where(x => x.Role.Name == Roles.WAITER_ROLE && x.Status == UserStatuses.USER_WORKED)
            .ToList();

        _tableComboBox.ItemsSource = _db.Tables.ToList();
    }

    private void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedWaiter = _waiterComboBox.SelectedItem as User;
        var selectedTable = _tableComboBox.SelectedItem as Table;

        if (selectedWaiter == null || selectedTable == null)
            return;

        var waiterTable = new WaiterTable { User = selectedWaiter, Table = selectedTable };
            
        Close(waiterTable);
    }

    private void CancelBtn_OnClick(object? sender, RoutedEventArgs e) => Close(null);
}