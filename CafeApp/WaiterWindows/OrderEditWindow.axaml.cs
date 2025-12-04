using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CafeApp.Helpers;
using CafeApp.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.WaiterWindows;

public partial class OrderEditWindow : Window
{
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    private ComboBox _tableComboBox;
    private ComboBox _statusComboBox;
    private TextBox _clientsAmountTextBox;
    private TextBox _contentTextBox;
    private TextBox _totalAmountTextBox;

    private Order? _editOrder;

    public List<Table> Tables { get; set; } = [];
    public List<string> Statuses { get; set; } = OrderStatuses.List;

    public OrderEditWindow()
    {
        InitializeComponent();
        
        _tableComboBox = this.FindControl<ComboBox>("TableComboBox")!;
        _statusComboBox = this.FindControl<ComboBox>("StatusComboBox")!;
        _clientsAmountTextBox = this.FindControl<TextBox>("ClientsAmountTextBox")!;
        _contentTextBox = this.FindControl<TextBox>("ContentTextBox")!;
        _totalAmountTextBox = this.FindControl<TextBox>("TotalAmountTextBox")!;

        LoadTables();
        LoadStatuses();
    }

    public OrderEditWindow(Order order) : this()
    {
        _editOrder = order;
        
        _tableComboBox.SelectedItem = _editOrder.Table;
        _statusComboBox.SelectedItem = _editOrder.Status;
        _clientsAmountTextBox.Text = _editOrder.ClientsAmount.ToString();
        _contentTextBox.Text = _editOrder.Content;
        _totalAmountTextBox.Text = _editOrder.TotalAmount.ToString();
    }

    private void LoadTables() => _tableComboBox.ItemsSource = _db.Tables.ToList();
    
    private void LoadStatuses() => _statusComboBox.ItemsSource = Statuses;

    private void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void CancelBtn_OnClick(object? sender, RoutedEventArgs e) => Close();
}