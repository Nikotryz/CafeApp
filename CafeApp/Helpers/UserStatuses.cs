using System.Collections.Generic;

namespace CafeApp;

public class UserStatuses
{
    public static List<string> List = [ USER_WORKED, USER_FIRED ];
    
    public const string USER_WORKED = "Работает";
    public const string USER_FIRED = "Уволен";
}