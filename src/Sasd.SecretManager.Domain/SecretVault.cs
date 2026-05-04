namespace Sasd.SecretManager.Domain;

/// <summary>
/// Fachliches Root-Objekt für einen einzelnen Tresor.
/// </summary>
public sealed class SecretVault
{
    /// <summary>
    /// Technische ID des Tresors.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Anzeigename des Tresors.
    /// </summary>
    public string Name { get; set; } = "Neuer Tresor";

    /// <summary>
    /// Formatversion des internen Modells.
    /// </summary>
    public int ModelVersion { get; set; } = 1;

    /// <summary>
    /// Alle fachlichen Gruppen innerhalb des Tresors.
    /// </summary>
    public List<EntryGroup> Groups { get; } = new();

    /// <summary>
    /// Alle Einträge innerhalb des Tresors.
    /// </summary>
    public List<SecretEntry> Entries { get; } = new();

    /// <summary>
    /// Globale bekannte Tags des Tresors.
    /// </summary>
    public List<string> KnownTags { get; } = new();
}
