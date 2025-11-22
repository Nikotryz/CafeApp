using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    
    private DataGrid usersDGrid;
    private DataGrid shiftsDGrid;

    private TextBox searchTBox;
    
    private Button editBtn;
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public AdminWindow()
    {
        InitializeComponent();

        searchTBox = this.FindControl<TextBox>("SearchTBox")!;
        
        usersDGrid = this.FindControl<DataGrid>("UsersDGrid")!;
        shiftsDGrid = this.FindControl<DataGrid>("ShiftsDGrid")!;
        
        editBtn = this.FindControl<Button>("EditBtn")!;

        usersDGrid.ItemsSource = _db.Users.Include(x => x.Role).ToList();
        shiftsDGrid.ItemsSource = _db.Shifts.Include(x => x.Users).Include(x => x.Orders).ToList();
    }

    private void SearchBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void DeleteBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private async void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        User? selectedUser = usersDGrid.SelectedItem as User;
        if (selectedUser != null)
        {
            await new UserEditWindow(selectedUser).ShowDialog(this);
            usersDGrid.ItemsSource = _db.Users.Include(x => x.Role).ToList();
        }
    }

    private void LogOutBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }

    private void UsersDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        editBtn.IsEnabled = usersDGrid.SelectedItem != null;
    }

    private void ShiftsDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedShift = shiftsDGrid.SelectedItem as Shift;
        if (selectedShift == null)
            return;
        
        new ShiftEditWindow(selectedShift).Show();
    }

    private void AddShiftBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        new ShiftEditWindow().ShowDialog(this);
        shiftsDGrid.ItemsSource = _db.Shifts.Include(x => x.Users).Include(x => x.Orders).ToList();
    }
}