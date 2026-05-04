using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Zentrale Anwendungslogik für Filterung, Suche und Sortierung von Einträgen.
/// Wir halten diese Logik bewusst aus der WinForms-Form heraus,
/// damit UI und Fachlogik nicht unnötig vermischt werden.
/// </summary>
public sealed class VaultQueryService
{
    /// <summary>
    /// Liefert Einträge passend zu Gruppe, Suche und Sortierung.
    /// </summary>
    public IReadOnlyList<SecretEntry> GetVisibleEntries(
        SecretVault vault,
        string? selectedGroupPath,
        string? searchText,
        int sortColumn,
        bool sortAscending)
    {
        ArgumentNullException.ThrowIfNull(vault);

        var groupPathMap = vault.Groups.ToDictionary(group => group.Id, group => group.Path);
        IEnumerable<SecretEntry> query = vault.Entries;

        if (!string.IsNullOrWhiteSpace(selectedGroupPath))
        {
            query = query.Where(entry => IsEntryInsideGroup(entry, groupPathMap, selectedGroupPath));
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(entry => MatchesSearch(entry, groupPathMap, searchText));
        }

        query = sortColumn switch
        {
            1 => sortAscending
                ? query.OrderBy(entry => entry.EntryType.ToString(), StringComparer.OrdinalIgnoreCase).ThenBy(entry => entry.Title, StringComparer.OrdinalIgnoreCase)
                : query.OrderByDescending(entry => entry.EntryType.ToString(), StringComparer.OrdinalIgnoreCase).ThenByDescending(entry => entry.Title, StringComparer.OrdinalIgnoreCase),
            2 => sortAscending
                ? query.OrderBy(entry => entry.UserName, StringComparer.OrdinalIgnoreCase).ThenBy(entry => entry.Title, StringComparer.OrdinalIgnoreCase)
                : query.OrderByDescending(entry => entry.UserName, StringComparer.OrdinalIgnoreCase).ThenByDescending(entry => entry.Title, StringComparer.OrdinalIgnoreCase),
            3 => sortAscending
                ? query.OrderBy(entry => string.Join(", ", entry.Tags), StringComparer.OrdinalIgnoreCase).ThenBy(entry => entry.Title, StringComparer.OrdinalIgnoreCase)
                : query.OrderByDescending(entry => string.Join(", ", entry.Tags), StringComparer.OrdinalIgnoreCase).ThenByDescending(entry => entry.Title, StringComparer.OrdinalIgnoreCase),
            _ => sortAscending
                ? query.OrderBy(entry => entry.Title, StringComparer.OrdinalIgnoreCase)
                : query.OrderByDescending(entry => entry.Title, StringComparer.OrdinalIgnoreCase),
        };

        return query.ToList();
    }

    /// <summary>
    /// Löst den sprechenden Gruppenpfad eines Eintrags auf.
    /// </summary>
    public string ResolveGroupPath(SecretVault vault, SecretEntry entry)
    {
        ArgumentNullException.ThrowIfNull(vault);
        ArgumentNullException.ThrowIfNull(entry);

        if (entry.GroupId is null)
        {
            return string.Empty;
        }

        var group = vault.Groups.FirstOrDefault(item => item.Id == entry.GroupId.Value);
        return group?.Path ?? string.Empty;
    }

    private static bool IsEntryInsideGroup(SecretEntry entry, IReadOnlyDictionary<Guid, string> groupPathMap, string selectedGroupPath)
    {
        if (entry.GroupId is null || !groupPathMap.TryGetValue(entry.GroupId.Value, out var groupPath))
        {
            return false;
        }

        return string.Equals(groupPath, selectedGroupPath, StringComparison.OrdinalIgnoreCase)
            || groupPath.StartsWith(selectedGroupPath + "/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchesSearch(SecretEntry entry, IReadOnlyDictionary<Guid, string> groupPathMap, string rawSearchText)
    {
        var searchText = rawSearchText.Trim();
        if (searchText.Length == 0)
        {
            return true;
        }

        var groupPath = string.Empty;
        if (entry.GroupId is Guid groupId && groupPathMap.TryGetValue(groupId, out var resolvedGroupPath))
        {
            groupPath = resolvedGroupPath;
        }

        // Für frühe Versionen reicht eine robuste Volltextsuche über die wichtigsten Felder.
        // Spätere Versionen können daraus eine strukturierte Query-Syntax machen.
        return Contains(entry.Title, searchText)
               || Contains(entry.UserName, searchText)
               || Contains(entry.Notes, searchText)
               || Contains(groupPath, searchText)
               || entry.Tags.Any(tag => Contains(tag, searchText))
               || entry.CustomFields.Any(field => Contains(field.Name, searchText) || Contains(field.Value, searchText));
    }

    private static bool Contains(string? value, string searchText)
    {
        return value?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true;
    }
}
