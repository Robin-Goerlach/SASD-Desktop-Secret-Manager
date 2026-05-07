// ============================================================================
// Dateiüberblick:
// Liefert die fachliche Schutzentscheidung für Copy-Aktionen. Die Klasse kennt
// bewusst keine WinForms- oder Clipboard-API, sondern beschreibt nur, welche
// Werte sensibel sind und wie diese behandelt werden sollen.
// ============================================================================

namespace Sasd.SecretManager.Security;

/// <summary>
/// Ermittelt die Schutzrichtlinie für Copy-Aktionen der Oberfläche.
/// </summary>
public sealed class ClipboardProtectionService
{
    private static readonly TimeSpan SensitiveClipboardLifetime = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Liefert die Schutzrichtlinie für die angegebene Copy-Aktion.
    /// </summary>
    /// <param name="copyKind">Die Art des zu kopierenden Werts.</param>
    /// <returns>
    /// Eine Policy, die festlegt, ob der Wert sensibel ist, automatisch gelöscht werden soll
    /// und welche Standardmeldungen die Oberfläche verwenden kann.
    /// </returns>
    public ClipboardCopyPolicy GetPolicy(ClipboardCopyKind copyKind)
    {
        return copyKind switch
        {
            ClipboardCopyKind.Secret => CreateSensitivePolicy(
                copiedMessage: "Secret in Zwischenablage kopiert.",
                clearedMessage: "Zwischenablage nach Secret-Kopie geleert."),

            ClipboardCopyKind.SecretCustomField => CreateSensitivePolicy(
                copiedMessage: "Geheimes Zusatzfeld in Zwischenablage kopiert.",
                clearedMessage: "Zwischenablage nach Kopie eines geheimen Zusatzfelds geleert."),

            ClipboardCopyKind.UserName => CreateStandardPolicy("Benutzername kopiert."),
            ClipboardCopyKind.Url => CreateStandardPolicy("URL kopiert."),
            ClipboardCopyKind.Host => CreateStandardPolicy("Host kopiert."),
            ClipboardCopyKind.Email => CreateStandardPolicy("E-Mail kopiert."),
            ClipboardCopyKind.Port => CreateStandardPolicy("Port kopiert."),
            ClipboardCopyKind.CustomField => CreateStandardPolicy("Zusatzfeld kopiert."),
            _ => CreateStandardPolicy("Wert kopiert."),
        };
    }

    private static ClipboardCopyPolicy CreateSensitivePolicy(string copiedMessage, string clearedMessage)
    {
        return new ClipboardCopyPolicy
        {
            IsSensitive = true,
            ShouldAutoClear = true,
            AutoClearDelay = SensitiveClipboardLifetime,
            CopiedMessage = copiedMessage,
            ClearedMessage = clearedMessage,
        };
    }

    private static ClipboardCopyPolicy CreateStandardPolicy(string copiedMessage)
    {
        return new ClipboardCopyPolicy
        {
            IsSensitive = false,
            ShouldAutoClear = false,
            AutoClearDelay = TimeSpan.Zero,
            CopiedMessage = copiedMessage,
            ClearedMessage = string.Empty,
        };
    }
}
