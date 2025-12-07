using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CafeApp.Models;

namespace CafeApp.Helpers;

public static class MyExtensions
{
    public static async Task<byte[]> ReadAllBytesAsync(this IStorageFile storageFile)
    {
        await using Stream stream = await storageFile.OpenReadAsync().ConfigureAwait(false);

        using var ms = new MemoryStream();

        await stream.CopyToAsync(ms).ConfigureAwait(false);

        return ms.ToArray();
    }

    public static Bitmap ToBitmap(this byte[] file)
    {
        using var ms = new MemoryStream(file);
        
        return new Bitmap(ms);
    }
    
    public static string ToStartAndEndTimeString(this Shift shift) => $"{shift.ShiftStarted.ToString("dd.MM.yyyy HH:mm")} - {shift.ShiftEnds.ToString("HH:mm")}";
}