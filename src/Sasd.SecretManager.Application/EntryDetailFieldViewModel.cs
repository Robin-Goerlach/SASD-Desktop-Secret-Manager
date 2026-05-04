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
