using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CafeApp.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.AdminWindows;

public partial class AddTableWindow : Window
{
    private readonly TextBox _numberTextBox;
    private readonly TextBlock _errorTextBlock;
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public AddTableWindow()
    {
        InitializeComponent();
        
        _numberTextBox = this.FindControl<TextBox>("NumberTextBox")!;
        _errorTextBlock = this.FindControl<TextBlock>("ErrorTextBlock")!;
    }

    private void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var allTables = _db.Tables.ToList();

        int number;
        try
        {
            number = Convert.ToInt32(_numberTextBox.Text);
        }
        catch (FormatException)
        {
            _errorTextBlock.Text = "Неправильный формат ввода";
            _errorTextBlock.IsVisible = true;
            return;
        }

        if (allTables.Any(x => x.Number == number))
        {
            _errorTextBlock.Text = "Такой столик уже существует";
            _errorTextBlock.IsVisible = true;
            return;
        }
        
        _db.Add(new Table { Number = number });
        _db.SaveChanges();
        
        Close();
    }

    private void CancelBtn_OnClick(object? sender, RoutedEventArgs e) => Close();
}