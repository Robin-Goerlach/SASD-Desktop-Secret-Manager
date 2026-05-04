using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Bearbeitungsmodell für einen Eintrag.
/// Dieses Modell hält nur editierbare Daten und keine UI-Steuerelemente.
/// </summary>
public sealed class EntryEditModel
{
    public string Title { get; set; } = string.Empty;
    public EntryType EntryType { get; set; } = EntryType.Login;
    public string UserName { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public string SelectedGroupPath { get; set; } = string.Empty;
    public string TagsText { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string CustomFieldsText { get; set; } = string.Empty;

    public static EntryEditModel CreateNew(string? preferredGroupPath)
    {
        return new EntryEditModel
        {
            SelectedGroupPath = preferredGroupPath ?? string.Empty,
            EntryType = EntryType.Login,
        };
    }

    public static EntryEditModel FromEntry(SecretEntry entry, string? groupPath)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var customFieldsText = string.Join(
            Environment.NewLine,
            entry.CustomFields
                .OrderBy(field => field.SortOrder)
                .ThenBy(field => field.Name, StringComparer.OrdinalIgnoreCase)
                .Select(field => $"{(field.IsSecret ? "!" : string.Empty)}{field.Name} = {field.Value}"));

        return new EntryEditModel
        {
            Title = entry.Title,
            EntryType = entry.EntryType,
            UserName = entry.UserName,
            Secret = entry.Secret,
            SelectedGroupPath = groupPath ?? string.Empty,
            // Wir normalisieren und sortieren Tags bereits hier so, dass ein
            // direktes Öffnen und unverändertes Speichern eines Eintrags nicht
            // fälschlich als fachliche Änderung erkannt wird.
            TagsText = entry.Tags.Count == 0
                ? string.Empty
                : string.Join(", ", entry.Tags.OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase)),
            Notes = entry.Notes,
            CustomFieldsText = customFieldsText,
        };
    }
}
