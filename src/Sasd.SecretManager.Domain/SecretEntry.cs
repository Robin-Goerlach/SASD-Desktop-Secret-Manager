namespace Sasd.SecretManager.Domain;

/// <summary>
/// Repräsentiert einen einzelnen fachlichen Eintrag innerhalb eines Tresors.
/// </summary>
public sealed class SecretEntry
{
    /// <summary>
    /// Eindeutige technische ID.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Kurzer sprechender Titel, z. B. "IONOS Webspace FTP".
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Fachlicher Typ des Eintrags.
    /// </summary>
    public EntryType EntryType { get; set; } = EntryType.Login;

    /// <summary>
    /// Primärer Benutzername, Login-Name oder Principal.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Primäres Secret wie Passwort oder Token.
    /// In frühen Modellständen bleibt das bewusst ein String.
    /// Die sichere Behandlung erfolgt später in Storage und UI.
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Freie Notizen zum Eintrag.
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Zugeordnete Hauptgruppe.
    /// </summary>
    public Guid? GroupId { get; set; }

    /// <summary>
    /// Flexible Tag-Liste für Querverbindungen und Suche.
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Frei definierbare Zusatzfelder.
    /// </summary>
    public List<CustomField> CustomFields { get; set; } = new();

    /// <summary>
    /// Erstellungszeitpunkt in UTC.
    /// </summary>
    public DateTimeOffset CreatedUtc { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Letzter Änderungszeitpunkt in UTC.
    /// </summary>
    public DateTimeOffset ModifiedUtc { get; set; } = DateTimeOffset.UtcNow;
}
