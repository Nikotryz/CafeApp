using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CafeApp.Models;
using OfficeOpenXml;

namespace CafeApp.Helpers;

public static class PKOFactory
{
    private const string PKO_TEMPLATE = "CafeApp.Resources.ПКО.xlsx";
    
    private const string ORGANIZATION = "ООО \"Кафе у дома\"";
    private const string ACCEPTED_FROM = "Харламов Н.В.";
    private const string REASON = "Возврат подотчетной суммы";
    private const string INCLUDING = "Без НДС";
    
    private const string CHIEF_ACCOUNTANT = "Окунев Е.Н.";
    private const string CASHIER = "Коньков Г.О.";

    public static async Task MakePKO(Order order, string pathToSave)
    {
        await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(PKO_TEMPLATE)!;

        using var package = new ExcelPackage(stream);
        
        var worksheet = package.Workbook.Worksheets.First();
        
        // ПКО
        worksheet.Cells["A7"].Value = ORGANIZATION;
        worksheet.Cells["AY7"].Value = 17356775;
        worksheet.Cells["AQ13"].Value = order.Id;
        worksheet.Cells["BB13"].Value = GetCurrentDateTime("dd.MM.yyyy");
        worksheet.Cells["A19"].Value = 50;
        worksheet.Cells["K19"].Value = "—";
        worksheet.Cells["T19"].Value = 71;
        worksheet.Cells["AD19"].Value = "—";
        worksheet.Cells["AO19"].Value = GetOrderTotalAmount(order, "F");
        worksheet.Cells["AX19"].Value = "—";
        worksheet.Cells["K21"].Value = ACCEPTED_FROM;
        worksheet.Cells["K23"].Value = REASON;
        worksheet.Cells["K28"].Value = INCLUDING;
        worksheet.Cells["AH32"].Value = CHIEF_ACCOUNTANT;
        worksheet.Cells["AH34"].Value = CASHIER;
        
        // Квитанция
        worksheet.Cells["BW3"].Value = ORGANIZATION;
        worksheet.Cells["CX9"].Value = order.Id;
        worksheet.Cells["CA10"].Value = GetCurrentDateTime("dd");
        worksheet.Cells["CG10"].Value = GetCurrentDateTime("MM");
        worksheet.Cells["CW10"].Value = GetCurrentDateTime("yyyy");
        worksheet.Cells["CF12"].Value = ACCEPTED_FROM;
        worksheet.Cells["CF14"].Value = REASON;
        
        var totalAmount = GetOrderTotalAmount(order, "F").Split(",");
        worksheet.Cells["CC19"].Value = totalAmount[0];
        worksheet.Cells["CY19"].Value = totalAmount[1];
        worksheet.Cells["CG25"].Value = INCLUDING;
        worksheet.Cells["BY27"].Value = GetCurrentDateTime("dd");
        worksheet.Cells["CE27"].Value = GetCurrentDateTime("MM");
        worksheet.Cells["CU27"].Value = GetCurrentDateTime("yyyy");
        worksheet.Cells["CW32"].Value = CHIEF_ACCOUNTANT;
        worksheet.Cells["CN34"].Value = CASHIER;
        
        await package.SaveAsAsync(pathToSave);
    }
    
    private static string GetCurrentDateTime(string format) => DateTime.Now.ToLocalTime().ToString(format);
    
    private static string GetOrderTotalAmount(Order order, string format) => order.TotalAmount.ToString(format, new CultureInfo("ru-RU"));
}