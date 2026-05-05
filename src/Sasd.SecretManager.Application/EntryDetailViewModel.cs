using Sasd.SecretManager.Domain;

// ============================================================================
// Dateiüberblick:
// Bündelt alle anzuzeigenden Detailinformationen eines Eintrags für die rechte Detailansicht.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

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
    /// <summary>
    /// Bereits für die Anzeige aufbereitete Tags.
    /// </summary>
    public IReadOnlyList<string> TagItems { get; init; } = Array.Empty<string>();
    public string SecretValue { get; init; } = string.Empty;
    public string SecretPreview { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public string PrimaryUrl { get; init; } = string.Empty;
    public string PrimaryHost { get; init; } = string.Empty;
    public string PrimaryEmail { get; init; } = string.Empty;
    public string PrimaryPort { get; init; } = string.Empty;
    /// <summary>
    /// Zusatzfelder, wie sie im Detailbereich und in Dialogen angezeigt werden sollen.
    /// </summary>
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
            TagItems = entry.Tags.OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase).ToArray(),
            SecretValue = entry.Secret,
            SecretPreview = string.IsNullOrWhiteSpace(entry.Secret) ? "(leer)" : "********",
            Notes = entry.Notes,
            PrimaryUrl = FindFirstField(entry.CustomFields, CustomFieldKind.Url, "url"),
            PrimaryHost = FindFirstField(entry.CustomFields, CustomFieldKind.Hostname, "host", "server"),
            PrimaryEmail = FindEmail(entry),
            PrimaryPort = FindFirstField(entry.CustomFields, CustomFieldKind.Port, "port"),
            CustomFields = customFields,
            CreatedUtc = entry.CreatedUtc,
            ModifiedUtc = entry.ModifiedUtc,
            CreatedDisplay = entry.CreatedUtc.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss"),
            ModifiedDisplay = entry.ModifiedUtc.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss"),
        };
    }

    private static string FindEmail(SecretEntry entry)
    {
        var fieldMail = FindFirstField(entry.CustomFields, CustomFieldKind.Email, "mail", "email");
        if (!string.IsNullOrWhiteSpace(fieldMail))
        {
            return fieldMail;
        }

        return entry.UserName.Contains('@') ? entry.UserName : string.Empty;
    }

    private static string FindFirstField(IReadOnlyList<CustomField> fields, CustomFieldKind expectedKind, params string[] nameHints)
    {
        var byKind = fields.FirstOrDefault(field => !field.IsSecret && field.Kind == expectedKind && !string.IsNullOrWhiteSpace(field.Value));
        if (byKind is not null)
        {
            return byKind.Value;
        }

        if (nameHints.Length > 0)
        {
            var byName = fields.FirstOrDefault(field =>
                !field.IsSecret &&
                !string.IsNullOrWhiteSpace(field.Value) &&
                nameHints.Any(hint => field.Name.Contains(hint, StringComparison.OrdinalIgnoreCase)));

            if (byName is not null)
            {
                return byName.Value;
            }
        }

        return string.Empty;
    }
}
