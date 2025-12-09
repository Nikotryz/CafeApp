namespace CafeApp.Helpers;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool IsValid(string? password, string? hashedPassword)
    {
        if (string.IsNullOrEmpty(password))
            return false;
        
        if (string.IsNullOrEmpty(hashedPassword))
            return false;
        
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}