using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CafeApp.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.AdminWindows;

public partial class AddWaiterTableWindow : Window
{
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    private readonly ComboBox _waiterComboBox;
    private readonly ComboBox _tableComboBox;

    private readonly Shift _shift;
    
    public List<User> Waiters { get; set; }
    public List<Table> Tables { get; set; }
    
    public AddWaiterTableWindow(Shift shift)
    {
        InitializeComponent();
        
        _waiterComboBox = this.FindControl<ComboBox>("WaiterComboBox")!;
        _tableComboBox = this.FindControl<ComboBox>("TableComboBox")!;
        
        _shift = shift;

        LoadData();
    }

    private void LoadData()
    {
        _waiterComboBox.ItemsSource = _db.Users
            .Where(x => x.Role.Name == Roles.WAITER_ROLE && x.Status == UserStatuses.USER_WORKED)
            .ToList();

        _tableComboBox.ItemsSource = _db.Tables.ToList();
    }

    private async void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedWaiter = _waiterComboBox.SelectedItem as User;
        var selectedTable = _tableComboBox.SelectedItem as Table;

        if (selectedWaiter == null || selectedTable == null)
            return;

        var existingWaiterTable = _db.WaiterTables
            .Where(x => x.Shift == _shift && x.User == selectedWaiter && x.Table == selectedTable)
            .FirstOrDefault();

        if (existingWaiterTable == null)
        {
            var waiterTable = new WaiterTable { Shift = _shift, User = selectedWaiter, Table = selectedTable };
            await _db.WaiterTables.AddAsync(waiterTable);
            await _db.SaveChangesAsync();
        }
        
        Close();
    }

    private void CancelBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}