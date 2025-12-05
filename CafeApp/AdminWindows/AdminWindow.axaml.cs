using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CafeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.AdminWindows;

public partial class AdminWindow : Window
{
    private readonly DataGrid _usersDGrid;
    private readonly DataGrid _shiftsDGrid;
    private readonly DataGrid _tablesDGrid;
    
    private readonly Button _userEditBtn;
    private readonly Button _shiftEditBtn;
    private readonly Button _userDeleteBtn;
    private readonly Button _shiftDeleteBtn;
    private readonly Button _deleteTableBtn;
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public List<User> UsersList { get; set; } = [];
    public List<Shift> ShiftsList { get; set; } = [];
    public List<Table> TablesList { get; set; } = [];
    
    public AdminWindow()
    {
        InitializeComponent();
        
        _usersDGrid = this.FindControl<DataGrid>("UsersDGrid")!;
        _shiftsDGrid = this.FindControl<DataGrid>("ShiftsDGrid")!;
        _tablesDGrid = this.FindControl<DataGrid>("TablesDGrid")!;
        
        _userEditBtn = this.FindControl<Button>("UserEditBtn")!;
        _shiftEditBtn = this.FindControl<Button>("ShiftEditBtn")!;
        _userDeleteBtn = this.FindControl<Button>("UserDeleteBtn")!;
        _shiftDeleteBtn = this.FindControl<Button>("ShiftDeleteBtn")!;
        _deleteTableBtn = this.FindControl<Button>("DeleteTableBtn")!;

        LoadUsers();
        LoadShifts();
        LoadTables();
    }

    private void LoadUsers()
    {
        _usersDGrid.ItemsSource = _db.Users
            .Include(x => x.Role)
            .ToList();
    }

    private void LoadShifts()
    {
        _shiftsDGrid.ItemsSource = _db.Shifts
            .Include(x => x.Users)
            .Include(x => x.Orders)
            .Include(x => x.WaiterTables)
            .ToList();
    } 
    
    private void LoadTables()
    {
        _tablesDGrid.ItemsSource = _db.Tables.ToList();
    }

    private void LogOutBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }

    private void UsersDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _userEditBtn.IsEnabled = _usersDGrid.SelectedItem != null;
        _userDeleteBtn.IsEnabled = _usersDGrid.SelectedItem != null;
    }
    
    private async void UserDeleteBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedUser = _usersDGrid.SelectedItem as User;
        if (selectedUser == null)
            return;
        
        _db.Users.Remove(selectedUser);
        await _db.SaveChangesAsync();
        LoadUsers();
    }

    private async void UserEditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedUser = _usersDGrid.SelectedItem as User;
        if (selectedUser == null)
            return;
        
        await new UserEditWindow(selectedUser).ShowDialog(this);
        LoadUsers();
    }
    
    private void AddUserBtn_OnClick(object? sender, RoutedEventArgs e) => new UserEditWindow().ShowDialog(this);

    private void ShiftsDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _shiftEditBtn.IsEnabled = _shiftsDGrid.SelectedItem != null;
        _shiftDeleteBtn.IsEnabled = _shiftsDGrid.SelectedItem != null;
    }
    
    private async void ShiftDeleteBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedShift = _shiftsDGrid.SelectedItem as Shift;
        if (selectedShift == null)
            return;
        
        _db.Shifts.Remove(selectedShift);
        await _db.SaveChangesAsync();
        LoadShifts();
    }

    private async void ShiftEditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedShift = _shiftsDGrid.SelectedItem as Shift;
        if (selectedShift == null)
            return;
        
        await new ShiftEditWindow(selectedShift).ShowDialog(this);
        LoadShifts();
    }
    private void AddShiftBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        new ShiftEditWindow().ShowDialog(this);
    }

    private void TablesDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => _deleteTableBtn.IsEnabled = _tablesDGrid.SelectedItem != null;

    private async void DeleteTableBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedTable = _tablesDGrid.SelectedItem as Table;
        if (selectedTable == null)
            return;
        
        _db.Tables.Remove(selectedTable);
        await _db.SaveChangesAsync();
        LoadTables();
    }

    private void AddTableBtn_OnClick(object? sender, RoutedEventArgs e) => new AddTableWindow().ShowDialog(this);
    
    private void RefreshUsersBtn_OnClick(object? sender, RoutedEventArgs e) => LoadUsers();

    private void RefreshShiftsBtn_OnClick(object? sender, RoutedEventArgs e) => LoadShifts();
    
    private void RefreshTablesBtn_OnClick(object? sender, RoutedEventArgs e) => LoadTables();
}