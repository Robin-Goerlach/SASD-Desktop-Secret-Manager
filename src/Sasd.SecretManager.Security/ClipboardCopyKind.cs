// ============================================================================
// Dateiüberblick:
// Beschreibt, welche Art von Wert in die Zwischenablage kopiert werden soll.
// Die Aufzählung dient als gemeinsame Sprache zwischen UI und Security-Schicht,
// damit sensible und nicht-sensible Copy-Aktionen konsistent behandelt werden.
// ============================================================================

namespace Sasd.SecretManager.Security;

/// <summary>
/// Beschreibt die fachliche Art einer Copy-Aktion.
/// </summary>
public enum ClipboardCopyKind
{
    /// <summary>
    /// Ein Benutzername oder Login-Name.
    /// </summary>
    UserName,

    /// <summary>
    /// Das primäre Passwort oder Secret eines Eintrags.
    /// </summary>
    Secret,

    /// <summary>
    /// Eine URL aus einem Eintrag.
    /// </summary>
    Url,

    /// <summary>
    /// Ein Hostname oder Servername.
    /// </summary>
    Host,

    /// <summary>
    /// Eine E-Mail-Adresse.
    /// </summary>
    Email,

    /// <summary>
    /// Ein Port-Wert oder eine Portangabe.
    /// </summary>
    Port,

    /// <summary>
    /// Ein normales Zusatzfeld ohne Geheimnischarakter.
    /// </summary>
    CustomField,

    /// <summary>
    /// Ein geheimes Zusatzfeld, etwa Token oder API-Key.
    /// </summary>
    SecretCustomField,
}
