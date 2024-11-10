using System;

namespace WiFiMapDataImporter;

public static class Logger
{
    public static void LogError(string message) => Log(message, ConsoleColor.Red);
    public static void LogUpdate(string message) => Log(message, ConsoleColor.Blue);
    public static void LogAdd(string message) => Log(message, ConsoleColor.Green);

    private static void Log(string message, ConsoleColor color)
    {
        ConsoleColor currentColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        Console.ForegroundColor = currentColor;
    }
}