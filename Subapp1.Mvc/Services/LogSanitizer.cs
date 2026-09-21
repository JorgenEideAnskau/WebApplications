namespace Subapp1.Mvc.Services;

public static class LogSanitizer
{
    public static string Clean(string? value)
        => (value ?? string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
}
