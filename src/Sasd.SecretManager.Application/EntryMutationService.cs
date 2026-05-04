using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Zuständig für das Erzeugen und Aktualisieren von Einträgen im laufenden In-Memory-Tresor.
/// Persistenz und Verschlüsselung folgen in späteren Meilensteinen.
/// </summary>
public sealed class EntryMutationService
{
    public IReadOnlyList<string> GetAvailableGroupPaths(SecretVault vault)
    {
        ArgumentNullException.ThrowIfNull(vault);

        return vault.Groups
            .Select(group => group.Path)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public SecretEntry CreateEntry(SecretVault vault, EntryEditModel model)
    {
        ArgumentNullException.ThrowIfNull(vault);
        ArgumentNullException.ThrowIfNull(model);

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

    public void UpdateEntry(SecretVault vault, SecretEntry entry, EntryEditModel model)
    {
        ArgumentNullException.ThrowIfNull(vault);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(model);

        entry.Title = model.Title.Trim();
        entry.EntryType = model.EntryType;
        entry.UserName = model.UserName.Trim();
        entry.Secret = model.Secret;
        entry.Notes = NormalizeMultiline(model.Notes);
        entry.GroupId = ResolveGroupId(vault, model.SelectedGroupPath);
        entry.ModifiedUtc = DateTimeOffset.UtcNow;

        entry.Tags.Clear();
        ApplyTags(entry, model.TagsText);

        entry.CustomFields.Clear();
        ApplyCustomFields(entry, model.CustomFieldsText);

        MergeKnownTags(vault, entry.Tags);
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
        if (string.IsNullOrWhiteSpace(tagsText))
        {
            return;
        }

        var tags = tagsText
            .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase);

        foreach (var tag in tags)
        {
            entry.Tags.Add(tag);
        }
    }

    private static void ApplyCustomFields(SecretEntry entry, string? customFieldsText)
    {
        if (string.IsNullOrWhiteSpace(customFieldsText))
        {
            return;
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
                continue;
            }

            var (rawName, value) = parts.Value;
            var isSecret = rawName.StartsWith('!');
            var name = rawName.TrimStart('!').Trim();
            if (name.Length == 0)
            {
                continue;
            }

            entry.CustomFields.Add(new CustomField
            {
                Name = name,
                Value = value.Trim(),
                IsSecret = isSecret,
                Kind = GuessKind(name, value, isSecret),
                SortOrder = sortOrder,
            });

            sortOrder += 10;
        }
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

        if (name.Contains("url", StringComparison.OrdinalIgnoreCase) || value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return CustomFieldKind.Url;
        }

        if (name.Contains("host", StringComparison.OrdinalIgnoreCase) || name.Contains("server", StringComparison.OrdinalIgnoreCase))
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
            : value.Replace("\r\n", "\n").Replace("\n", Environment.NewLine).Trim();
    }
}
