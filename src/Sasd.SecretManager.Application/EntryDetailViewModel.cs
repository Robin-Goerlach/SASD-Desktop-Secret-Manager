using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Lesefreundliche strukturierte Ansicht eines Eintrags für die Hauptoberfläche.
/// </summary>
public sealed class EntryDetailViewModel
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string EntryType { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string GroupPath { get; init; } = string.Empty;
    public string Tags { get; init; } = string.Empty;
    public string SecretValue { get; init; } = string.Empty;
    public string SecretPreview { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public IReadOnlyList<EntryDetailFieldViewModel> CustomFields { get; init; } = Array.Empty<EntryDetailFieldViewModel>();
    public DateTimeOffset CreatedUtc { get; init; }
    public DateTimeOffset ModifiedUtc { get; init; }
    public string CreatedDisplay { get; init; } = string.Empty;
    public string ModifiedDisplay { get; init; } = string.Empty;

    /// <summary>
    /// Erzeugt eine Detailansicht aus einem Domain-Eintrag.
    /// Das Secret wird im ViewModel bewusst vollständig vorgehalten,
    /// damit die UI es maskiert oder sichtbar darstellen kann.
    /// </summary>
    public static EntryDetailViewModel FromEntry(SecretEntry entry, string? groupPath)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var customFields = entry.CustomFields
            .OrderBy(field => field.SortOrder)
            .ThenBy(field => field.Name, StringComparer.OrdinalIgnoreCase)
            .Select(field => new EntryDetailFieldViewModel
            {
                Name = field.Name,
                Value = field.Value,
                DisplayValue = field.IsSecret ? "********" : field.Value,
                IsSecret = field.IsSecret,
                Kind = field.Kind.ToString(),
            })
            .ToArray();

        return new EntryDetailViewModel
        {
            Id = entry.Id,
            Title = entry.Title,
            EntryType = entry.EntryType.ToString(),
            UserName = entry.UserName,
            GroupPath = string.IsNullOrWhiteSpace(groupPath) ? "(keine Gruppe)" : groupPath,
            Tags = entry.Tags.Count == 0 ? string.Empty : string.Join(", ", entry.Tags),
            SecretValue = entry.Secret,
            SecretPreview = string.IsNullOrWhiteSpace(entry.Secret) ? "(leer)" : "********",
            Notes = entry.Notes,
            CustomFields = customFields,
            CreatedUtc = entry.CreatedUtc,
            ModifiedUtc = entry.ModifiedUtc,
            CreatedDisplay = entry.CreatedUtc.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss"),
            ModifiedDisplay = entry.ModifiedUtc.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss"),
        };
    }
}
