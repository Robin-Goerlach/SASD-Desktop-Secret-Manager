using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Zuständig für das Erzeugen und Aktualisieren von Einträgen im laufenden
/// In-Memory-Tresor.
/// </summary>
/// <remarks>
/// DSM-003 ergänzt eine zentrale fachliche Validierung. Der Service ist damit
/// nicht mehr darauf angewiesen, dass ausschließlich die WinForms-Oberfläche
/// korrekt prüft. Auch spätere Importer oder Automatisierungen laufen dadurch
/// durch dieselben Regeln.
/// </remarks>
public sealed class EntryMutationService
{
    private readonly EntryValidationService _validationService;

    /// <summary>
    /// Erstellt den Service mit der Standardvalidierung.
    /// </summary>
    public EntryMutationService()
        : this(new EntryValidationService())
    {
    }

    /// <summary>
    /// Erstellt den Service mit explizit übergebener Validierung.
    /// </summary>
    /// <remarks>
    /// Diese Überladung ist vor allem für Tests und spätere Dependency-Injection-
    /// Szenarien gedacht. Für die aktuelle WinForms-Anwendung reicht der
    /// parameterlose Konstruktor.
    /// </remarks>
    public EntryMutationService(EntryValidationService validationService)
    {
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    }

    /// <summary>
    /// Liefert alle bekannten Gruppenpfade für Drop-downs und Dialoge.
    /// </summary>
    public IReadOnlyList<string> GetAvailableGroupPaths(SecretVault vault)
    {
        ArgumentNullException.ThrowIfNull(vault);

        return vault.Groups
            .Select(group => group.Path)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    /// <summary>
    /// Erstellt einen neuen Eintrag aus einem Bearbeitungsmodell und hängt ihn an
    /// den Tresor an.
    /// </summary>
    /// <exception cref="EntryValidationException">
    /// Wird geworfen, wenn Pflichtfelder, Zusatzfelder, Gruppe oder Eindeutigkeit
    /// nicht gültig sind.
    /// </exception>
    public SecretEntry CreateEntry(SecretVault vault, EntryEditModel model)
    {
        ArgumentNullException.ThrowIfNull(vault);
        ArgumentNullException.ThrowIfNull(model);

        _validationService.ValidateForCreate(vault, model).ThrowIfInvalid();

        var entry = new SecretEntry
        {
            Title = model.Title.Trim(),
            EntryType = model.EntryType,
            UserName = model.UserName.Trim(),
            Secret = model.Secret,
            Notes = NormalizeMultiline(model.Notes),
            GroupId = ResolveGroupId(vault, model.SelectedGroupPath),
            ModifiedUtc = DateTimeOffset.UtcNow,
        };

        ApplyTags(entry, model.TagsText);
        ApplyCustomFields(entry, model.CustomFieldsText);
        MergeKnownTags(vault, entry.Tags);
        vault.Entries.Add(entry);
        return entry;
    }

    /// <summary>
    /// Aktualisiert einen Eintrag und liefert zurück, ob sich fachlich tatsächlich
    /// etwas geändert hat.
    /// </summary>
    /// <exception cref="EntryValidationException">
    /// Wird geworfen, wenn die geänderten Daten nicht gespeichert werden dürfen.
    /// </exception>
    public bool UpdateEntry(SecretVault vault, SecretEntry entry, EntryEditModel model)
    {
        ArgumentNullException.ThrowIfNull(vault);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(model);

        _validationService.ValidateForUpdate(vault, entry, model).ThrowIfInvalid();

        var normalizedTitle = model.Title.Trim();
        var normalizedUserName = model.UserName.Trim();
        var normalizedNotes = NormalizeMultiline(model.Notes);
        var normalizedGroupId = ResolveGroupId(vault, model.SelectedGroupPath);
        var normalizedTags = ParseTags(model.TagsText);
        var normalizedCustomFields = ParseCustomFields(model.CustomFieldsText);

        var changed = !string.Equals(entry.Title, normalizedTitle, StringComparison.Ordinal)
            || entry.EntryType != model.EntryType
            || !string.Equals(entry.UserName, normalizedUserName, StringComparison.Ordinal)
            || !string.Equals(entry.Secret, model.Secret, StringComparison.Ordinal)
            || !string.Equals(entry.Notes, normalizedNotes, StringComparison.Ordinal)
            || entry.GroupId != normalizedGroupId
            || !AreTagsEqual(entry.Tags, normalizedTags)
            || !AreCustomFieldsEqual(entry.CustomFields, normalizedCustomFields);

        if (!changed)
        {
            return false;
        }

        entry.Title = normalizedTitle;
        entry.EntryType = model.EntryType;
        entry.UserName = normalizedUserName;
        entry.Secret = model.Secret;
        entry.Notes = normalizedNotes;
        entry.GroupId = normalizedGroupId;
        entry.ModifiedUtc = DateTimeOffset.UtcNow;

        entry.Tags.Clear();
        entry.Tags.AddRange(normalizedTags);

        entry.CustomFields.Clear();
        entry.CustomFields.AddRange(normalizedCustomFields);

        MergeKnownTags(vault, entry.Tags);
        return true;
    }

    private static Guid? ResolveGroupId(SecretVault vault, string? groupPath)
    {
        if (string.IsNullOrWhiteSpace(groupPath))
        {
            return null;
        }

        var group = vault.Groups.FirstOrDefault(item => string.Equals(item.Path, groupPath, StringComparison.OrdinalIgnoreCase));
        return group?.Id;
    }

    private static void ApplyTags(SecretEntry entry, string? tagsText)
    {
        foreach (var tag in ParseTags(tagsText))
        {
            entry.Tags.Add(tag);
        }
    }

    private static List<string> ParseTags(string? tagsText)
    {
        if (string.IsNullOrWhiteSpace(tagsText))
        {
            return new List<string>();
        }

        return tagsText
            .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static void ApplyCustomFields(SecretEntry entry, string? customFieldsText)
    {
        foreach (var field in ParseCustomFields(customFieldsText))
        {
            entry.CustomFields.Add(field);
        }
    }

    private static List<CustomField> ParseCustomFields(string? customFieldsText)
    {
        var fields = new List<CustomField>();
        if (string.IsNullOrWhiteSpace(customFieldsText))
        {
            return fields;
        }

        var lines = customFieldsText
            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var sortOrder = 10;
        foreach (var line in lines)
        {
            if (line.StartsWith('#'))
            {
                continue;
            }

            var parts = SplitCustomFieldLine(line);
            if (parts is null)
            {
                // Ungültige Zeilen werden seit DSM-003 vorher validiert. Diese
                // Schutzabfrage bleibt dennoch als defensive Sicherung erhalten,
                // falls der Parser später außerhalb des normalen Speicherwegs
                // verwendet wird.
                continue;
            }

            var (rawName, value) = parts.Value;
            var isSecret = rawName.TrimStart().StartsWith('!');
            var name = rawName.TrimStart().TrimStart('!').Trim();

            if (name.Length == 0)
            {
                continue;
            }

            fields.Add(new CustomField
            {
                Name = name,
                Value = value.Trim(),
                IsSecret = isSecret,
                Kind = GuessKind(name, value, isSecret),
                SortOrder = sortOrder,
            });
            sortOrder += 10;
        }

        return fields;
    }

    private static (string Name, string Value)? SplitCustomFieldLine(string line)
    {
        var separatorIndex = line.IndexOf('=');
        if (separatorIndex < 0)
        {
            separatorIndex = line.IndexOf(':');
        }

        if (separatorIndex <= 0)
        {
            return null;
        }

        var name = line[..separatorIndex];
        var value = line[(separatorIndex + 1)..];
        return (name, value);
    }

    private static CustomFieldKind GuessKind(string name, string value, bool isSecret)
    {
        if (isSecret)
        {
            return CustomFieldKind.Secret;
        }

        if (name.Contains("url", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return CustomFieldKind.Url;
        }

        if (name.Contains("host", StringComparison.OrdinalIgnoreCase)
            || name.Contains("server", StringComparison.OrdinalIgnoreCase))
        {
            return CustomFieldKind.Hostname;
        }

        if (name.Contains("port", StringComparison.OrdinalIgnoreCase) && int.TryParse(value, out _))
        {
            return CustomFieldKind.Port;
        }

        if (name.Contains("mail", StringComparison.OrdinalIgnoreCase) || value.Contains('@'))
        {
            return CustomFieldKind.Email;
        }

        if (bool.TryParse(value, out _))
        {
            return CustomFieldKind.Boolean;
        }

        if (decimal.TryParse(value, out _))
        {
            return CustomFieldKind.Number;
        }

        return CustomFieldKind.Text;
    }

    private static bool AreTagsEqual(IReadOnlyList<string> left, IReadOnlyList<string> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        // Tags werden bei der Eingabe normalisiert und alphabetisch sortiert. Für
        // den Vergleich eines unverändert geöffneten und wieder gespeicherten
        // Eintrags müssen wir daher auch die bereits vorhandenen Tags in dieselbe
        // Vergleichsform bringen, statt ihre ursprüngliche Reihenfolge als
        // fachlich relevant zu behandeln.
        var normalizedLeft = left
            .OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var normalizedRight = right
            .OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        for (var index = 0; index < normalizedLeft.Length; index++)
        {
            if (!string.Equals(normalizedLeft[index], normalizedRight[index], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    private static bool AreCustomFieldsEqual(IReadOnlyList<CustomField> left, IReadOnlyList<CustomField> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        for (var index = 0; index < left.Count; index++)
        {
            var leftField = left[index];
            var rightField = right[index];

            if (!string.Equals(leftField.Name, rightField.Name, StringComparison.Ordinal)
                || !string.Equals(leftField.Value, rightField.Value, StringComparison.Ordinal)
                || leftField.IsSecret != rightField.IsSecret
                || leftField.Kind != rightField.Kind
                || leftField.SortOrder != rightField.SortOrder)
            {
                return false;
            }
        }

        return true;
    }

    private static void MergeKnownTags(SecretVault vault, IEnumerable<string> tags)
    {
        foreach (var tag in tags)
        {
            if (!vault.KnownTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
            {
                vault.KnownTags.Add(tag);
            }
        }

        vault.KnownTags.Sort(StringComparer.OrdinalIgnoreCase);
    }

    private static string NormalizeMultiline(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace("\n", Environment.NewLine, StringComparison.Ordinal)
                .Trim();
    }
}
