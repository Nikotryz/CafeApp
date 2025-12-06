using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;
using Aspose.Cells;

namespace CafeApp.Helpers;

public static class ExcelToPdfConverter
{
    public static async Task SaveAsPdfAsync(this ExcelPackage package, string pathToSave)
    {
        await package.SaveAsAsync(pathToSave);
        
        var workbook = new Workbook(pathToSave);
        
        foreach (Worksheet worksheet in workbook.Worksheets)
        {
            PageSetup pageSetup = worksheet.PageSetup;
            pageSetup.Orientation = PageOrientationType.Landscape;
        }
        
        File.Delete(pathToSave);
        
        var pathWithoutType = pathToSave.Split('.')[0];
        
        await workbook.SaveAsync($"{pathWithoutType}.pdf");
    }
}