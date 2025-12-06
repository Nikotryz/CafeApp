using System.Linq;
using Avalonia.Controls;
using CafeApp.Helpers;
using CafeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.WaiterWindows;

public partial class WaiterReportWindow : Window
{
    private readonly TextBlock _reportTextBlock;
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public WaiterReportWindow(Shift shift)
    {
        InitializeComponent();
        
        _reportTextBlock =  this.FindControl<TextBlock>("ReportTextBlock")!;

        _reportTextBlock.Text = MakeReport(shift);
    }

    private string MakeReport(Shift shift)
    {
        var orders = _db.Orders
            .Include(x => x.Shift)
            .Include(x => x.Table)
            .Where(x => x.Shift == shift)
            .ToList();
        
        return $"""
               Заказов за смену: {orders.Count}
               Кол-во заказов, оплаченных наличными: {orders.Where(x => x.PaymentMethod == PaymentMethods.CASH).ToList().Count}
               Кол-во заказов, оплаченных безналичным способом: {orders.Where(x => x.PaymentMethod == PaymentMethods.NON_CASH).ToList().Count}
               Общая выручка: {orders.Sum(x => x.TotalAmount)}
                 наличными - {orders.Where(x => x.PaymentMethod == PaymentMethods.CASH).Sum(x => x.TotalAmount)}
                 безналичным способом - {orders.Where(x => x.PaymentMethod == PaymentMethods.NON_CASH).Sum(x => x.TotalAmount)}
               Общее кол-во клиентов: {orders.Sum(x => x.ClientsAmount)}
               """;
    }
}