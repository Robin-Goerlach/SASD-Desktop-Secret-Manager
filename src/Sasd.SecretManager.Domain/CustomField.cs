// ============================================================================
// Dateiüberblick:
// Beschreibt ein frei definierbares Zusatzfeld eines Eintrags.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.Domain;

/// <summary>
/// Repräsentiert ein frei definierbares Zusatzfeld innerhalb eines Eintrags.
/// </summary>
public sealed class CustomField
{
    /// <summary>
    /// Technische ID des Felds.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Anzeigename des Felds, z. B. "Host" oder "Datenbankname".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Fachlicher Typ des Felds.
    /// </summary>
    public CustomFieldKind Kind { get; set; } = CustomFieldKind.Text;

    /// <summary>
    /// Gespeicherter Feldwert.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Kennzeichnet, ob das Feld in der UI standardmäßig verborgen werden sollte.
    /// </summary>
    public bool IsSecret { get; set; }

    /// <summary>
    /// Erlaubt eine stabile Sortierung der Zusatzfelder.
    /// </summary>
    public int SortOrder { get; set; }
}
