using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CafeApp.Helpers;
using CafeApp.Models;
using CafeApp.WaiterWindows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.CookWindows;

public partial class CookWindow : Window
{
    private readonly DataGrid _ordersDataGrid;

    private readonly TextBlock _currentShiftTextBlock;
    private readonly Button _editOrderBtn;
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();

    public Shift CurrentShift { get; set; } = null!;
    public List<Order> Orders { get; set; } = [];
    
    public CookWindow()
    {
        InitializeComponent();
        
        _ordersDataGrid =  this.FindControl<DataGrid>("OrdersDataGrid")!;
        _currentShiftTextBlock = this.FindControl<TextBlock>("CurrentShiftTextBlock")!;
        _editOrderBtn = this.FindControl<Button>("EditOrderBtn")!;
        
        CurrentShift = GetCurrentShift();
        
        _currentShiftTextBlock.Text = GetCurrentShift().ShiftStarted.ToString("dd.MM.yyyy HH:mm");

        LoadOrders();
    }
    
    private void LoadOrders()
    {
        _ordersDataGrid.ItemsSource = _db.Orders
            .Include(x => x.Shift)
            .Include(x => x.Table)
            .ToList();
    }
    
    private void OrdersDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _editOrderBtn.IsEnabled = _ordersDataGrid.SelectedItem != null;
    }
    
    private void RefreshOrdersBtn_OnClick(object? sender, RoutedEventArgs e) => LoadOrders();
    
    private void EditOrderBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedOrder = _ordersDataGrid.SelectedItem as Order;
        if (selectedOrder != null)
            new OrderEditWindow(selectedOrder, CurrentShift, GetUserRole()).ShowDialog(this);
    }

    private void LogOutBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }
    
    private Shift GetCurrentShift() => _db.Shifts.First(x => x.ShiftStarted <= DateTime.Now && x.ShiftEnds >= DateTime.Now);
    
    private Role GetUserRole() => _db.Roles.First(x => x.Name == Roles.COOK_ROLE);
}