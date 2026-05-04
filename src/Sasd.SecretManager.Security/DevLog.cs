using System.Diagnostics;
using System.Text;

namespace Sasd.SecretManager.Security;

/// <summary>
/// Kleine Debug-Hilfsklasse für Entwicklungs-Logs.
/// Zusätzlich kann in Debug-Builds eine temporäre Session-Logdatei geschrieben werden,
/// damit Meldungen bei Abstürzen oder schnellem Beenden nicht verloren gehen.
/// </summary>
public static class DevLog
{
    private static readonly object SyncRoot = new();
    private static string? _sessionLogPath;

    /// <summary>
    /// Pfad der aktuellen Session-Logdatei.
    /// In Release-Builds ist dieser Wert leer.
    /// </summary>
    public static string SessionLogPath => _sessionLogPath ?? string.Empty;

    /// <summary>
    /// Initialisiert eine neue Debug-Session-Logdatei.
    /// Die alte Datei wird beim Start bewusst gelöscht, damit immer nur die letzte
    /// Ausführung im Log steht und kein unendlicher Verlauf entsteht.
    /// </summary>
    [Conditional("DEBUG")]
    public static void InitializeSessionLog()
    {
        try
        {
            var directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Sasd.SecretManager",
                "Logs");

            Directory.CreateDirectory(directory);

            _sessionLogPath = Path.Combine(directory, "debug-session.log");

            if (File.Exists(_sessionLogPath))
            {
                File.Delete(_sessionLogPath);
            }

            File.WriteAllText(
                _sessionLogPath,
                $"=== SASD Secret Manager Debug Session {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==={Environment.NewLine}",
                Encoding.UTF8);
        }
        catch
        {
            // Logging darf die Anwendung niemals blockieren.
        }
    }

    /// <summary>
    /// Schreibt eine Meldung nur in Debug-Builds.
    /// </summary>
    [Conditional("DEBUG")]
    public static void WriteLine(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var line = $"[DEBUG {timestamp}] {message}";

        Console.WriteLine(line);
        Debug.WriteLine(line);
        AppendToSessionLog(line);
    }

    /// <summary>
    /// Schreibt eine Ausnahme mit Kontext in das Debug-Log.
    /// </summary>
    [Conditional("DEBUG")]
    public static void WriteException(string context, Exception exception)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var line = $"[ERROR {timestamp}] {context}: {exception}";

        Console.WriteLine(line);
        Debug.WriteLine(line);
        AppendToSessionLog(line);
    }

    private static void AppendToSessionLog(string line)
    {
#if DEBUG
        if (string.IsNullOrWhiteSpace(_sessionLogPath))
        {
            return;
        }

        try
        {
            lock (SyncRoot)
            {
                File.AppendAllText(_sessionLogPath, line + Environment.NewLine, Encoding.UTF8);
            }
        }
        catch
        {
            // Logging darf die Anwendung niemals blockieren.
        }
#endif
    }
}
