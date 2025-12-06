using System.Collections.Generic;

namespace CafeApp.Helpers;

public static class OrderStatuses
{
    public static List<string> List = [ ACCEPTED, COOKING, COMPLETED ];
    
    public const string ACCEPTED = "Принят";
    public const string COOKING = "Готовится";
    public const string COMPLETED = "Выполнен";
}