// ============================================================================
// Dateiüberblick:
// Kapselt die Schutzentscheidung für eine Copy-Aktion. Die UI fragt diese
// Policy ab und entscheidet dann, ob ein Timer gestartet und welche Meldung
// dem Nutzer angezeigt werden soll.
// ============================================================================

namespace Sasd.SecretManager.Security;

/// <summary>
/// Enthält die fachliche Richtlinie für eine Copy-Aktion.
/// </summary>
public sealed class ClipboardCopyPolicy
{
    /// <summary>
    /// Gibt an, ob der kopierte Wert als sensibel behandelt werden soll.
    /// </summary>
    public bool IsSensitive { get; init; }

    /// <summary>
    /// Gibt an, ob der kopierte Wert nach einer Frist automatisch wieder aus der Zwischenablage entfernt werden soll.
    /// </summary>
    public bool ShouldAutoClear { get; init; }

    /// <summary>
    /// Die Wartezeit bis zum automatischen Leeren der Zwischenablage.
    /// </summary>
    public TimeSpan AutoClearDelay { get; init; } = TimeSpan.Zero;

    /// <summary>
    /// Die Standardmeldung, die nach erfolgreichem Kopieren angezeigt werden soll.
    /// </summary>
    public string CopiedMessage { get; init; } = string.Empty;

    /// <summary>
    /// Die Standardmeldung, die nach einem automatischen Leeren angezeigt werden soll.
    /// </summary>
    public string ClearedMessage { get; init; } = string.Empty;
}
