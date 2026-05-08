namespace Sasd.SecretManager.Application;

/// <summary>
/// Beschreibt vordefinierte Spezialfilter für die Eintragsliste.
/// </summary>
/// <remarks>
/// Diese Filter sind bewusst fachlich und nicht UI-spezifisch formuliert.
/// Dadurch kann dieselbe Filterlogik später auch für Importberichte,
/// Konsolenwerkzeuge oder automatisierte Tests wiederverwendet werden.
/// </remarks>
public enum EntrySpecialFilter
{
    /// <summary>
    /// Kein Spezialfilter. Alle Einträge bleiben in der Ergebnismenge.
    /// </summary>
    None = 0,

    /// <summary>
    /// Zeigt nur Einträge, die keiner Gruppe zugeordnet sind.
    /// </summary>
    WithoutGroup = 1,

    /// <summary>
    /// Zeigt nur Einträge mit einem URL-Zusatzfeld.
    /// </summary>
    WithUrlField = 2,

    /// <summary>
    /// Zeigt nur Einträge mit einem Hostnamen- oder Host-Zusatzfeld.
    /// </summary>
    WithHostField = 3,

    /// <summary>
    /// Zeigt nur Einträge mit einem E-Mail-Zusatzfeld.
    /// </summary>
    WithEmailField = 4,

    /// <summary>
    /// Zeigt nur Einträge, die überhaupt Zusatzfelder besitzen.
    /// </summary>
    WithCustomFields = 5,

    /// <summary>
    /// Zeigt nur Einträge mit geheimen Zusatzfeldern.
    /// </summary>
    WithSecretCustomFields = 6,
}
