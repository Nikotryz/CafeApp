using CafeApp.Models;

namespace CafeApp;

public class CafeDbService
{
    private static CafeDbContext? _db;

    public static CafeDbContext GetDbContext()
    {
        if (_db == null)
            _db = new CafeDbContext();
        
        return _db;
    }
}