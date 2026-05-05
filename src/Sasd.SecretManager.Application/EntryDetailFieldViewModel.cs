// ============================================================================
// Dateiüberblick:
// Beschreibt ein einzelnes Feld für die Detailansicht eines Eintrags. Die ViewModel-Schicht formatiert Domain-Daten so, dass die UI keine Fachlogik kennen muss.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.Application;

/// <summary>
/// Strukturierte Darstellung eines einzelnen Zusatzfelds im Detailbereich.
/// </summary>
public sealed class EntryDetailFieldViewModel
{
    public string Name { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string DisplayValue { get; init; } = string.Empty;
    public bool IsSecret { get; init; }
    public string Kind { get; init; } = string.Empty;
}
