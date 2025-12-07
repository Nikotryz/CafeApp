using System.Collections.Generic;

namespace CafeApp.Helpers;

public static class OrderStatuses
{
    public static readonly  List<string> List = [ ACCEPTED, COOKING, COOKED, COMPLETED, PAID ];
    public static readonly List<string> WaiterList = [ ACCEPTED, COMPLETED, PAID ];
    public static readonly List<string> CookList = [ COOKING, COOKED ];
    
    public static Dictionary<string, List<string>> AvailableStatuses = new Dictionary<string, List<string>>
    {
        {Roles.ADMIN_ROLE, List},
        {Roles.WAITER_ROLE, WaiterList},
        {Roles.COOK_ROLE, CookList}
    };
    
    public const string ACCEPTED = "Принят";
    public const string COOKING = "Готовится";
    public const string COOKED = "Готов";
    public const string COMPLETED = "Выполнен";
    public const string PAID = "Оплачен";
}