using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CafeApp.Helpers;
using CafeApp.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.WaiterWindows;

public partial class OrderEditWindow : Window
{
    private readonly ComboBox _tableComboBox;
    private readonly ComboBox _statusComboBox;
    private readonly ComboBox _paymentMethodComboBox;
    private readonly TextBox _clientsAmountTextBox;
    private readonly TextBox _contentTextBox;
    private readonly TextBox _totalAmountTextBox;
    
    private readonly TextBlock _errorTextBlock;
    
    private readonly Button _saveBtn;
    private readonly Button _cancelBtn;

    private readonly Order? _editOrder;
    private readonly Shift _currentShift;
    private readonly Role _currentRole;
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public bool IsWaiter { get; set; }
    public List<Table> Tables { get; set; } = [];
    public List<string> Statuses { get; set; } = OrderStatuses.List;
    public List<string> PaymentMethods { get; set; } = CafeApp.Helpers.PaymentMethods.List;

    public OrderEditWindow(Shift currentShift, Role role)
    {
        InitializeComponent();
        
        _tableComboBox = this.FindControl<ComboBox>("TableComboBox")!;
        _statusComboBox = this.FindControl<ComboBox>("StatusComboBox")!;
        _paymentMethodComboBox = this.FindControl<ComboBox>("PaymentMethodComboBox")!;
        _clientsAmountTextBox = this.FindControl<TextBox>("ClientsAmountTextBox")!;
        _contentTextBox = this.FindControl<TextBox>("ContentTextBox")!;
        _totalAmountTextBox = this.FindControl<TextBox>("TotalAmountTextBox")!;
        
        _errorTextBlock = this.FindControl<TextBlock>("ErrorTextBlock")!;
        
        _saveBtn = this.FindControl<Button>("SaveBtn")!;
        _cancelBtn = this.FindControl<Button>("CancelBtn")!;

        _currentShift = currentShift;
        _currentRole = role;
        IsWaiter = role.Name == Roles.WAITER_ROLE;

        EnableFields();
        
        LoadTables();
        LoadStatuses();
        LoadPaymentMethods();
    }

    public OrderEditWindow(Order order, Shift currentShift, Role role) : this(currentShift, role)
    {
        _editOrder = order;
        
        _tableComboBox.SelectedItem = _editOrder.Table;
        _statusComboBox.SelectedItem = _editOrder.Status;
        _paymentMethodComboBox.SelectedItem = _editOrder.PaymentMethod;
        _clientsAmountTextBox.Text = _editOrder.ClientsAmount.ToString();
        _contentTextBox.Text = _editOrder.Content;
        _totalAmountTextBox.Text = _editOrder.TotalAmount.ToString("F", new CultureInfo("ru-RU"));
    }

    private void EnableFields()
    {
        _tableComboBox.IsEnabled = IsWaiter;
        _paymentMethodComboBox.IsEnabled = IsWaiter;
        _clientsAmountTextBox.IsEnabled = IsWaiter;
        _contentTextBox.IsEnabled = IsWaiter;
        _totalAmountTextBox.IsEnabled = IsWaiter;
        
        _statusComboBox.IsEnabled = _currentRole.Name != Roles.ADMIN_ROLE;
        
        _saveBtn.IsVisible = _currentRole.Name != Roles.ADMIN_ROLE;
        _cancelBtn.Content = _currentRole.Name != Roles.ADMIN_ROLE ?  "Отмена" : "OK";
    }

    private void LoadTables() => _tableComboBox.ItemsSource = _db.Tables.ToList();
    
    private void LoadStatuses() => _statusComboBox.ItemsSource = Statuses;
    
    private void LoadPaymentMethods() => _paymentMethodComboBox.ItemsSource = PaymentMethods;

    private void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var order = _editOrder ?? new Order();

        if (_tableComboBox.SelectedItem is Table table &&
            _statusComboBox.SelectedItem is string status &&
            _paymentMethodComboBox.SelectedItem is string paymentMethod &&
            !string.IsNullOrWhiteSpace(_clientsAmountTextBox.Text) &&
            !string.IsNullOrWhiteSpace(_contentTextBox.Text) &&
            !string.IsNullOrWhiteSpace(_totalAmountTextBox.Text))
        {
            try
            {
                order.ClientsAmount = int.Parse(_clientsAmountTextBox.Text);
            }
            catch (FormatException)
            {
                ShowMessage("Число клиентов в неправильном формате");
                return;
            }
            try
            {
                order.TotalAmount = decimal.Parse(_totalAmountTextBox.Text);
            }
            catch (FormatException)
            {
                ShowMessage("Итоговая сумма в неправильном формате");
                return;
            }
            order.Table = table;
            order.Shift = _currentShift;
            order.Status = status;
            order.PaymentMethod = paymentMethod;
            order.Content = _contentTextBox.Text;
        }
        else
        {
            ShowMessage("Не все поля заполнены");
            return;
        }

        if (_editOrder != null)
        {
            if (order.Status == OrderStatuses.COMPLETED)
                order.CompletedAt = TimeOnly.FromDateTime(DateTime.Now.ToLocalTime());
            _db.Update(order);
        }
        else
        {
            order.CreatedAt = TimeOnly.FromDateTime(DateTime.Now.ToLocalTime());
            _db.Add(order);
        }
        
        _db.SaveChanges();
        
        Close();
    }

    private void CancelBtn_OnClick(object? sender, RoutedEventArgs e) => Close();

    private void ShowMessage(string message)
    {
        _errorTextBlock.Text = message;
        _errorTextBlock.IsVisible = true;
    }
}