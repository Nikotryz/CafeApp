using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CafeApp.Helpers;
using CafeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.WaiterWindows;

public partial class WaiterWindow : Window
{
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    private readonly DataGrid _ordersDataGrid;

    private readonly TextBlock _currentShiftTextBlock;
    private readonly Button _deleteOrderBtn;
    private readonly Button _editOrderBtn;

    public Shift? CurrentShift { get; set; }
    public List<Order> Orders { get; set; } = [];
    
    public WaiterWindow()
    {
        InitializeComponent();
        
        _ordersDataGrid = this.FindControl<DataGrid>("OrdersDataGrid")!;
        _deleteOrderBtn = this.FindControl<Button>("DeleteOrderBtn")!;
        _editOrderBtn = this.FindControl<Button>("EditOrderBtn")!;
        _currentShiftTextBlock = this.FindControl<TextBlock>("CurrentShiftTextBlock")!;
        
        CurrentShift = GetCurrentShift();

        _currentShiftTextBlock.Text = GetCurrentShift()?.ShiftStarted.ToString("dd.MM.yyyy HH:mm");
        
        LoadOrders();
    }

    private void LoadOrders()
    {
        _ordersDataGrid.ItemsSource = _db.Orders
            .Include(x => x.Shift)
            .Include(x => x.Table)
            .ToList();
    }

    private void LogOutBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }

    private void OrdersDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _deleteOrderBtn.IsEnabled = _ordersDataGrid.SelectedItem != null;
        _editOrderBtn.IsEnabled = _ordersDataGrid.SelectedItem != null;
    }
    
    private void RefreshOrdersBtn_OnClick(object? sender, RoutedEventArgs e) => LoadOrders();

    private void DeleteOrderBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedOrder = _ordersDataGrid.SelectedItem as Order;
        if (selectedOrder == null)
            return;
        
        _db.Orders.Remove(selectedOrder);
        _db.SaveChanges();
    }

    private void EditOrderBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedOrder = _ordersDataGrid.SelectedItem as Order;
        if (selectedOrder != null)
            new OrderEditWindow(selectedOrder, CurrentShift, GetUserRole()).ShowDialog(this);
    }

    private void AddOrderBtn_OnClick(object? sender, RoutedEventArgs e) => new OrderEditWindow(CurrentShift, GetUserRole()).ShowDialog(this);
    
    private Shift? GetCurrentShift() => _db.Shifts.FirstOrDefault(x => x.ShiftStarted <= DateTime.Now && x.ShiftEnds >= DateTime.Now);

    private Role GetUserRole() => _db.Roles.First(x => x.Name == Roles.WAITER_ROLE);
}