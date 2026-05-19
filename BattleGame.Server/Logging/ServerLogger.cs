using System.Globalization;

namespace BattleGame.Server.Logging;

public static class ServerLogger
{
    private static readonly object Sync = new();
    private static readonly LogLevel MinimumLevel = ParseLogLevel(
        Environment.GetEnvironmentVariable("LOG_LEVEL"),
        LogLevel.Info);

    private static readonly string ServiceName =
        FirstNonEmpty(
            Environment.GetEnvironmentVariable("LOG_SERVICE_NAME"),
            Environment.GetEnvironmentVariable("SERVER_ID"),
            "server");

    public static bool IsDebugEnabled => MinimumLevel <= LogLevel.Debug;

    public static bool InputPacketsEnabled { get; } = ParseBool(
        Environment.GetEnvironmentVariable("LOG_INPUT_PACKETS"));

    public static void Info(string message, string category = "app")
    {
        Write(LogLevel.Info, category, message);
    }

    public static void Warn(string message, string category = "app")
    {
        Write(LogLevel.Warn, category, message);
    }

    public static void Error(string message, string category = "app")
    {
        Write(LogLevel.Error, category, message);
    }

    public static void Debug(string message, string category = "app")
    {
        Write(LogLevel.Debug, category, message);
    }

    public static void Event(string category, string action, params (string Key, object? Value)[] fields)
    {
        string suffix = fields.Length == 0
            ? string.Empty
            : " " + string.Join(" ", fields.Select(field => $"{field.Key}={FormatValue(field.Value)}"));

        Info($"action={action}{suffix}", category);
    }

    private static void Write(LogLevel level, string category, string message)
    {
        if (level < MinimumLevel)
            return;

        string timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture);
        string safeCategory = Normalize(category, 12);
        string safeService = Normalize(ServiceName, 12);
        string safeMessage = (message ?? string.Empty)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);

        lock (Sync)
        {
            Console.WriteLine($"{timestamp} | {level.ToString().ToUpperInvariant(),-5} | {safeService,-12} | {safeCategory,-12} | {safeMessage}");
        }
    }

    private static string Normalize(string value, int maxLength)
    {
        string normalized = string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        return normalized.Length <= maxLength ? normalized : normalized[..maxLength];
    }

    private static string FormatValue(object? value)
    {
        if (value == null)
            return "-";

        string text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? "-";
        text = text.Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal)
            .Trim();

        return text.Any(char.IsWhiteSpace) ? $"\"{text.Replace("\"", "\\\"", StringComparison.Ordinal)}\"" : text;
    }

    private static LogLevel ParseLogLevel(string? raw, LogLevel fallback)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return fallback;

        return raw.Trim().ToUpperInvariant() switch
        {
            "DEBUG" => LogLevel.Debug,
            "INFO" => LogLevel.Info,
            "WARN" or "WARNING" => LogLevel.Warn,
            "ERROR" => LogLevel.Error,
            _ => fallback
        };
    }

    private static bool ParseBool(string? raw)
    {
        return !string.IsNullOrWhiteSpace(raw) &&
            raw.Trim().ToLowerInvariant() is "1" or "true" or "yes" or "on";
    }

    private static string FirstNonEmpty(params string?[] values)
    {
        foreach (string? value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }

        return "server";
    }

    private enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3
    }
}
