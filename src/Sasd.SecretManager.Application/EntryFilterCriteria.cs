using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Fasst alle Filter- und Sortierparameter für die Eintragsliste zusammen.
/// </summary>
/// <remarks>
/// Vor DSM-004 wurden Suchtext, Gruppenpfad und Sortierung als einzelne
/// Parameter an <see cref="VaultQueryService"/> übergeben. Für strukturierte
/// Filter wäre diese Signatur schnell unübersichtlich geworden. Dieses kleine
/// Kriterienobjekt hält die Anwendungsschicht sauber, testbar und später
/// erweiterbar, ohne die WinForms-Oberfläche mit Fachlogik zu überladen.
/// </remarks>
public sealed class EntryFilterCriteria
{
    /// <summary>
    /// Optionaler Gruppenpfad aus dem TreeView. Untergruppen werden mit umfasst.
    /// </summary>
    public string? SelectedGroupPath { get; init; }

    /// <summary>
    /// Freier Suchtext für die bestehende Volltextsuche.
    /// </summary>
    public string? SearchText { get; init; }

    /// <summary>
    /// Optionaler fachlicher Eintragstyp, zum Beispiel Login, Server oder API Key.
    /// </summary>
    public EntryType? EntryType { get; init; }

    /// <summary>
    /// Optionaler Tag-Filter. Der Vergleich erfolgt exakt, aber ohne Beachtung
    /// von Groß-/Kleinschreibung.
    /// </summary>
    public string? Tag { get; init; }

    /// <summary>
    /// Optionaler Spezialfilter für häufige fachliche Sichten.
    /// </summary>
    public EntrySpecialFilter SpecialFilter { get; init; } = EntrySpecialFilter.None;

    /// <summary>
    /// Index der ListView-Spalte, nach der sortiert werden soll.
    /// </summary>
    public int SortColumn { get; init; }

    /// <summary>
    /// Gibt an, ob die Sortierung aufsteigend erfolgen soll.
    /// </summary>
    public bool SortAscending { get; init; } = true;

    /// <summary>
    /// Gibt an, ob abseits von Gruppe und Volltextsuche mindestens ein
    /// strukturierter Filter aktiv ist.
    /// </summary>
    public bool HasStructuredFilters => EntryType.HasValue
        || !string.IsNullOrWhiteSpace(Tag)
        || SpecialFilter != EntrySpecialFilter.None;

    /// <summary>
    /// Gibt an, ob überhaupt ein Filter aktiv ist.
    /// </summary>
    public bool HasAnyFilter => HasStructuredFilters
        || !string.IsNullOrWhiteSpace(SelectedGroupPath)
        || !string.IsNullOrWhiteSpace(SearchText);

    /// <summary>
    /// Erstellt eine bereinigte Kopie des Kriterienobjekts.
    /// </summary>
    /// <remarks>
    /// Die UI darf Leerzeichen oder leere Strings liefern. Die Application-
    /// Schicht normalisiert diese Werte, damit alle Tests und späteren Aufrufer
    /// dieselben Regeln verwenden.
    /// </remarks>
    public EntryFilterCriteria Normalize()
    {
        return new EntryFilterCriteria
        {
            SelectedGroupPath = NormalizeOptionalText(SelectedGroupPath),
            SearchText = NormalizeOptionalText(SearchText),
            EntryType = EntryType,
            Tag = NormalizeOptionalText(Tag),
            SpecialFilter = SpecialFilter,
            SortColumn = SortColumn,
            SortAscending = SortAscending,
        };
    }

    private static string? NormalizeOptionalText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}
