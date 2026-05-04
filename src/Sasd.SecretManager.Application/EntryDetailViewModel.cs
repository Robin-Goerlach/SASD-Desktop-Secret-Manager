using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Lesefreundliche Ansicht eines Eintrags für PropertyGrid und Dialoge.
/// Das Ziel ist eine ruhige, verständliche Darstellung ohne interne Listenobjekte.
/// </summary>
public sealed class EntryDetailViewModel
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string EntryType { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string GroupPath { get; init; } = string.Empty;
    public string Tags { get; init; } = string.Empty;
    public string SecretPreview { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public string CustomFields { get; init; } = string.Empty;
    public DateTimeOffset CreatedUtc { get; init; }
    public DateTimeOffset ModifiedUtc { get; init; }

    /// <summary>
    /// Erzeugt eine Detailansicht aus einem Domain-Eintrag.
    /// Das Secret wird absichtlich nur maskiert dargestellt.
    /// </summary>
    public static EntryDetailViewModel FromEntry(SecretEntry entry, string? groupPath)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var customFieldLines = entry.CustomFields
            .OrderBy(field => field.SortOrder)
            .ThenBy(field => field.Name, StringComparer.OrdinalIgnoreCase)
            .Select(field => $"{field.Name}: {(field.IsSecret ? "********" : field.Value)}")
            .ToArray();

        return new EntryDetailViewModel
        {
            Id = entry.Id,
            Title = entry.Title,
            EntryType = entry.EntryType.ToString(),
            UserName = entry.UserName,
            GroupPath = groupPath ?? "(keine Gruppe)",
            Tags = entry.Tags.Count == 0 ? string.Empty : string.Join(", ", entry.Tags),
            SecretPreview = string.IsNullOrWhiteSpace(entry.Secret) ? "(leer)" : "********",
            Notes = entry.Notes,
            CustomFields = customFieldLines.Length == 0 ? "(keine Zusatzfelder)" : string.Join(Environment.NewLine, customFieldLines),
            CreatedUtc = entry.CreatedUtc,
            ModifiedUtc = entry.ModifiedUtc,
        };
    }
}
