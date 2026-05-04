using System.Runtime.InteropServices;

namespace Sasd.SecretManager.Security;

/// <summary>
/// Öffnet in Debug-Builds eine Windows-Konsole für Entwicklungs-Logs.
/// </summary>
public static class DebugConsole
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    private static bool _isInitialized;

    /// <summary>
    /// Initialisiert die Debug-Konsole einmalig.
    /// </summary>
    public static void Initialize()
    {
#if DEBUG
        if (_isInitialized)
        {
            return;
        }

        // In Debug möchten wir eine Konsole ähnlich zu vielen Linux-GUI-Programmen haben,
        // damit technische Meldungen sichtbar bleiben, ohne die eigentliche Oberfläche zu stören.
        AllocConsole();
        _isInitialized = true;
        DevLog.WriteLine("Debug-Konsole initialisiert.");
#endif
    }
}
