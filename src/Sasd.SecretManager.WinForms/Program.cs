using Sasd.SecretManager.Security;

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
        DevLog.WriteLine("Programmstart.");

        ApplicationConfiguration.Initialize();
        System.Windows.Forms.Application.Run(new MainForm());
    }
}
