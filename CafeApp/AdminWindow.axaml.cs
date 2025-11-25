using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CafeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp;

public partial class AdminWindow : Window
{
    public List<User> UsersList { get; set; }
    public List<Shift> ShiftsList { get; set; }
    
    private readonly DataGrid _usersDGrid;
    private readonly DataGrid _shiftsDGrid;

    private readonly TextBox _searchTBox;
    
    private readonly Button _userEditBtn;
    private readonly Button _shiftEditBtn;
    private readonly Button _userDeleteBtn;
    private readonly Button _shiftDeleteBtn;
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public AdminWindow()
    {
        InitializeComponent();

        _searchTBox = this.FindControl<TextBox>("SearchTBox")!;
        
        _usersDGrid = this.FindControl<DataGrid>("UsersDGrid")!;
        _shiftsDGrid = this.FindControl<DataGrid>("ShiftsDGrid")!;
        
        _userEditBtn = this.FindControl<Button>("UserEditBtn")!;
        _shiftEditBtn = this.FindControl<Button>("ShiftEditBtn")!;
        _userDeleteBtn = this.FindControl<Button>("UserDeleteBtn")!;
        _shiftDeleteBtn = this.FindControl<Button>("ShiftDeleteBtn")!;

        LoadUsers();
        LoadShifts();
    }

    private void LoadUsers()
    {
        _usersDGrid.ItemsSource = _db.Users.Include(x => x.Role).ToList();
    }

    private void LoadShifts()
    {
        _shiftsDGrid.ItemsSource = _db.Shifts.Include(x => x.Users).Include(x => x.Orders).ToList();
    } 

    // TODO
    private void SearchBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
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
    
    private void AddUserBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        new UserEditWindow().ShowDialog(this);
        LoadUsers();
    }

    private void ShiftsDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _shiftEditBtn.IsEnabled = _shiftsDGrid.SelectedItem != null;
        _shiftDeleteBtn.IsEnabled = _shiftsDGrid.SelectedItem != null;
    }

    private void AddShiftBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        new ShiftEditWindow().ShowDialog(this);
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

    private async void ShiftDeleteBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedShift = _shiftsDGrid.SelectedItem as Shift;
        if (selectedShift == null)
            return;
        
        _db.Shifts.Remove(selectedShift);
        await _db.SaveChangesAsync();
        LoadShifts();
    }
}