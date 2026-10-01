namespace Task_05___Debug___Refactor_Pack.Services;

public static class ValidationHelper
{
    public static string RequireNotEmpty(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

        return value.Trim();
    }

    public static decimal RequirePositive(decimal value, string fieldName)
    {
        if (value <= 0)
            throw new ArgumentException($"{fieldName} must be positive.", fieldName);

        return value;
    }

    public static int RequirePositive(int value, string fieldName)
    {
        if (value <= 0)
            throw new ArgumentException($"{fieldName} must be positive.", fieldName);

        return value;
    }
}
