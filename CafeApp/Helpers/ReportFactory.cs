using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CafeApp.Models;
using OfficeOpenXml;

namespace CafeApp.Helpers;

public static class ReportFactory
{
    private const string ADMIN_REPORT_TEMPLATE = "CafeApp.Resources.AdminReport.xlsx";

    public static async Task MakeReport(List<Order> orders, string pathToSave, bool isPdf = false)
    {
        await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ADMIN_REPORT_TEMPLATE)!;

        using var package = new ExcelPackage(stream);
        
        var worksheet = package.Workbook.Worksheets.First();

        for (int row = 2; row < orders.Count + 2; row++)
        {
            var order = orders[row - 2];
            
            worksheet.Cells[$"A{row}"].Value = order.Id;
            worksheet.Cells[$"B{row}"].Value = order.Table.Number;
            worksheet.Cells[$"C{row}"].Value = order.ClientsAmount;
            worksheet.Cells[$"D{row}"].Value = order.Content;
            worksheet.Cells[$"E{row}"].Value = order.TotalAmount.ToString("F", new CultureInfo("ru-RU"));
            worksheet.Cells[$"F{row}"].Value = order.Status;
            worksheet.Cells[$"G{row}"].Value = order.CreatedAt.ToString("HH:mm");
            worksheet.Cells[$"H{row}"].Value = order.CompletedAt?.ToString("HH:mm") ?? "Не выполнен";
            worksheet.Cells[$"I{row}"].Value = order.PaymentMethod;
        }
        
        if (isPdf)
            await package.SaveAsPdfAsync(pathToSave);
        else
            await package.SaveAsAsync(pathToSave);
    }
}