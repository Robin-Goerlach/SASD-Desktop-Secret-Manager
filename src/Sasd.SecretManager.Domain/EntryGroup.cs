namespace Sasd.SecretManager.Domain;

/// <summary>
/// Beschreibt eine hierarchische Gruppe innerhalb eines Tresors.
/// </summary>
public sealed class EntryGroup
{
    /// <summary>
    /// Eindeutige ID der Gruppe.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Anzeigename der Gruppe.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optionale Eltern-ID für verschachtelte Gruppen.
    /// </summary>
    public Guid? ParentGroupId { get; set; }

    /// <summary>
    /// Optionaler beschreibender Pfad, der später zur Anzeige oder Suche genutzt werden kann.
    /// </summary>
    public string Path { get; set; } = string.Empty;
}
