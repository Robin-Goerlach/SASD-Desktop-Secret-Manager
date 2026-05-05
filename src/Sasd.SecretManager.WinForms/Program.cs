using System.Windows.Forms;
using Sasd.SecretManager.Security;

// ============================================================================
// Dateiüberblick:
// Anwendungseinstiegspunkt mit Debug-Initialisierung und globaler Ausnahmeverdrahtung.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.WinForms;

internal static class Program
{
    /// <summary>
    /// Anwendungseinstiegspunkt.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // In Debug-Builds öffnen wir bewusst zusätzlich eine Konsole,
        // damit Entwicklungs-Logs wie bei vielen Linux-GUI-Anwendungen sichtbar bleiben.
        DebugConsole.Initialize();
        DevLog.InitializeSessionLog();
        DevLog.WriteLine("Programmstart.");

        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
        {
            if (eventArgs.ExceptionObject is Exception exception)
            {
                DevLog.WriteException("Nicht behandelte Domain-Ausnahme", exception);
            }
        };

        System.Windows.Forms.Application.ThreadException += (_, eventArgs) =>
        {
            DevLog.WriteException("Nicht behandelte UI-Ausnahme", eventArgs.Exception);

            var details = string.IsNullOrWhiteSpace(DevLog.SessionLogPath)
                ? "Bitte die Debug-Ausgaben prüfen."
                : $"Das Debug-Log wurde hier geschrieben:{Environment.NewLine}{DevLog.SessionLogPath}";

            MessageBox.Show(
                $"Es ist ein unerwarteter Fehler aufgetreten.{Environment.NewLine}{Environment.NewLine}{details}",
                "SASD Secret Manager",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        };

        ApplicationConfiguration.Initialize();
        System.Windows.Forms.Application.Run(new MainForm());
    }
}
