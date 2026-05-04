using System.Diagnostics;

namespace Sasd.SecretManager.Security;

/// <summary>
/// Kleine Debug-Hilfsklasse für Entwicklungs-Logs.
/// </summary>
public static class DevLog
{
    /// <summary>
    /// Schreibt eine Meldung nur in Debug-Builds.
    /// </summary>
    [Conditional("DEBUG")]
    public static void WriteLine(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        Console.WriteLine($"[DEBUG {timestamp}] {message}");
        Debug.WriteLine($"[DEBUG {timestamp}] {message}");
    }
}
