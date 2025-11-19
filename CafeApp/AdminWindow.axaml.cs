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
    
    private TextBox searchTBox;
    private DataGrid usersDGrid;

    private Button editBtn;
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public AdminWindow()
    {
        InitializeComponent();

        searchTBox = this.FindControl<TextBox>("SearchTBox")!;
        usersDGrid = this.FindControl<DataGrid>("UsersDGrid")!;
        editBtn = this.FindControl<Button>("EditBtn")!;

        usersDGrid.ItemsSource = _db.Users.Include(x => x.Role).ToList();
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
        throw new System.NotImplementedException();
    }

    private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        editBtn.IsEnabled = usersDGrid.SelectedItem != null;
    }
}