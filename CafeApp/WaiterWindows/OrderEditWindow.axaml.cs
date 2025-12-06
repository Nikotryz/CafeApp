using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
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

    private readonly Order? _editOrder;
    private readonly Shift _currentShift;
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    public List<Table> Tables { get; set; } = [];
    public List<string> Statuses { get; set; } = OrderStatuses.List;
    public List<string> PaymentMethods { get; set; } = CafeApp.Helpers.PaymentMethods.List;

    public OrderEditWindow(Shift currentShift)
    {
        InitializeComponent();
        
        _tableComboBox = this.FindControl<ComboBox>("TableComboBox")!;
        _statusComboBox = this.FindControl<ComboBox>("StatusComboBox")!;
        _paymentMethodComboBox = this.FindControl<ComboBox>("PaymentMethodComboBox")!;
        _clientsAmountTextBox = this.FindControl<TextBox>("ClientsAmountTextBox")!;
        _contentTextBox = this.FindControl<TextBox>("ContentTextBox")!;
        _totalAmountTextBox = this.FindControl<TextBox>("TotalAmountTextBox")!;
        
        _errorTextBlock = this.FindControl<TextBlock>("ErrorTextBlock")!;

        _currentShift = currentShift;

        EnableFields();
        
        LoadTables();
        LoadStatuses();
        LoadPaymentMethods();
    }

    public OrderEditWindow(Order order, Shift currentShift) : this(currentShift)
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
        var isAdminOrWaiter = App.CurrentUser.Role.Name == Roles.WAITER_ROLE ||
                              App.CurrentUser.Role.Name == Roles.ADMIN_ROLE;
        
        _tableComboBox.IsEnabled = isAdminOrWaiter;
        _paymentMethodComboBox.IsEnabled = isAdminOrWaiter;
        _clientsAmountTextBox.IsEnabled = isAdminOrWaiter;
        _contentTextBox.IsEnabled = isAdminOrWaiter;
        _totalAmountTextBox.IsEnabled = isAdminOrWaiter;
    }

    private void LoadTables() => _tableComboBox.ItemsSource = _db.Tables.ToList();
    
    private void LoadStatuses() => _statusComboBox.ItemsSource = Statuses;
    
    private void LoadPaymentMethods() => _paymentMethodComboBox.ItemsSource = PaymentMethods;

    private async void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
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
        
        if (order.CompletedAt == null && (order.Status == OrderStatuses.COMPLETED || order.Status == OrderStatuses.PAID))
            order.CompletedAt = TimeOnly.FromDateTime(DateTime.Now.ToLocalTime());

        if (_editOrder != null)
        {
            _db.Update(order);
        }
        else
        {
            order.CreatedAt = TimeOnly.FromDateTime(DateTime.Now.ToLocalTime());
            _db.Add(order);
        }
        
        await _db.SaveChangesAsync();
        
        if (order.Status == OrderStatuses.PAID)
        {
            var pathToSave = await GetPathToSaveAsync(order);
            if (pathToSave != null)
                await PKOFactory.MakePKO(order, pathToSave);
        }
        
        Close();
    }

    private void CancelBtn_OnClick(object? sender, RoutedEventArgs e) => Close();

    private async Task<string?> GetPathToSaveAsync(Order order)
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Сохранить ПКО",
            SuggestedFileName = $"ПКО_№{order.Id}.xlsx",
            FileTypeChoices = new[] { new FilePickerFileType("Excel Files") { Patterns = ["*.xlsx"] } }
        });
        
        return file?.TryGetLocalPath();
    }

    private void ShowMessage(string message)
    {
        _errorTextBlock.Text = message;
        _errorTextBlock.IsVisible = true;
    }
}