using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Zentrale Anwendungslogik für Filterung, Suche und Sortierung von Einträgen.
/// </summary>
/// <remarks>
/// Die WinForms-Oberfläche sammelt nur Benutzereingaben ein. Welche Einträge
/// fachlich sichtbar sind, entscheidet dieser Service. Dadurch bleibt die
/// Filterlogik gut testbar und kann später auch für Importberichte, CLI-Tools
/// oder weitere Oberflächen genutzt werden.
/// </remarks>
public sealed class VaultQueryService
{
    /// <summary>
    /// Liefert Einträge passend zu Gruppe, Suche und Sortierung.
    /// </summary>
    /// <remarks>
    /// Diese Überladung bleibt für vorhandenen Code erhalten und leitet auf das
    /// neue DSM-004-Kriterienobjekt weiter.
    /// </remarks>
    public IReadOnlyList<SecretEntry> GetVisibleEntries(
        SecretVault vault,
        string? selectedGroupPath,
        string? searchText,
        int sortColumn,
        bool sortAscending)
    {
        return GetVisibleEntries(vault, new EntryFilterCriteria
        {
            SelectedGroupPath = selectedGroupPath,
            SearchText = searchText,
            SortColumn = sortColumn,
            SortAscending = sortAscending,
        });
    }

    /// <summary>
    /// Liefert alle sichtbaren Einträge für die übergebenen Filterkriterien.
    /// </summary>
    /// <param name="vault">Der aktuell geöffnete Tresor.</param>
    /// <param name="criteria">Filter- und Sortierkriterien aus der UI oder aus Tests.</param>
    /// <returns>Eine bereits sortierte, materialisierte Ergebnisliste.</returns>
    public IReadOnlyList<SecretEntry> GetVisibleEntries(SecretVault vault, EntryFilterCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(vault);
        ArgumentNullException.ThrowIfNull(criteria);

        var normalizedCriteria = criteria.Normalize();
        var groupPathMap = vault.Groups.ToDictionary(group => group.Id, group => group.Path);

        IEnumerable<SecretEntry> query = vault.Entries;

        if (!string.IsNullOrWhiteSpace(normalizedCriteria.SelectedGroupPath))
        {
            query = query.Where(entry => IsEntryInsideGroup(entry, groupPathMap, normalizedCriteria.SelectedGroupPath));
        }

        if (normalizedCriteria.EntryType.HasValue)
        {
            query = query.Where(entry => entry.EntryType == normalizedCriteria.EntryType.Value);
        }

        if (!string.IsNullOrWhiteSpace(normalizedCriteria.Tag))
        {
            query = query.Where(entry => entry.Tags.Any(tag => string.Equals(tag, normalizedCriteria.Tag, StringComparison.OrdinalIgnoreCase)));
        }

        query = ApplySpecialFilter(query, normalizedCriteria.SpecialFilter);

        if (!string.IsNullOrWhiteSpace(normalizedCriteria.SearchText))
        {
            query = query.Where(entry => MatchesSearch(entry, groupPathMap, normalizedCriteria.SearchText));
        }

        query = ApplySorting(query, normalizedCriteria.SortColumn, normalizedCriteria.SortAscending);
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

    private static IEnumerable<SecretEntry> ApplySpecialFilter(IEnumerable<SecretEntry> query, EntrySpecialFilter filter)
    {
        return filter switch
        {
            EntrySpecialFilter.None => query,
            EntrySpecialFilter.WithoutGroup => query.Where(entry => entry.GroupId is null),
            EntrySpecialFilter.WithUrlField => query.Where(entry => HasCustomFieldKindOrName(entry, CustomFieldKind.Url, "url", "website", "webseite", "link")),
            EntrySpecialFilter.WithHostField => query.Where(entry => HasCustomFieldKindOrName(entry, CustomFieldKind.Hostname, "host", "hostname", "server")),
            EntrySpecialFilter.WithEmailField => query.Where(entry => HasCustomFieldKindOrName(entry, CustomFieldKind.Email, "email", "e-mail", "mail")),
            EntrySpecialFilter.WithCustomFields => query.Where(entry => entry.CustomFields.Count > 0),
            EntrySpecialFilter.WithSecretCustomFields => query.Where(entry => entry.CustomFields.Any(field => field.IsSecret || field.Kind == CustomFieldKind.Secret)),
            _ => query,
        };
    }

    private static IEnumerable<SecretEntry> ApplySorting(IEnumerable<SecretEntry> query, int sortColumn, bool sortAscending)
    {
        return sortColumn switch
        {
            1 => sortAscending
                ? query.OrderBy(entry => entry.EntryType.ToString(), StringComparer.OrdinalIgnoreCase)
                    .ThenBy(entry => entry.Title, StringComparer.OrdinalIgnoreCase)
                : query.OrderByDescending(entry => entry.EntryType.ToString(), StringComparer.OrdinalIgnoreCase)
                    .ThenByDescending(entry => entry.Title, StringComparer.OrdinalIgnoreCase),
            2 => sortAscending
                ? query.OrderBy(entry => entry.UserName, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(entry => entry.Title, StringComparer.OrdinalIgnoreCase)
                : query.OrderByDescending(entry => entry.UserName, StringComparer.OrdinalIgnoreCase)
                    .ThenByDescending(entry => entry.Title, StringComparer.OrdinalIgnoreCase),
            3 => sortAscending
                ? query.OrderBy(entry => string.Join(", ", entry.Tags), StringComparer.OrdinalIgnoreCase)
                    .ThenBy(entry => entry.Title, StringComparer.OrdinalIgnoreCase)
                : query.OrderByDescending(entry => string.Join(", ", entry.Tags), StringComparer.OrdinalIgnoreCase)
                    .ThenByDescending(entry => entry.Title, StringComparer.OrdinalIgnoreCase),
            _ => sortAscending
                ? query.OrderBy(entry => entry.Title, StringComparer.OrdinalIgnoreCase)
                : query.OrderByDescending(entry => entry.Title, StringComparer.OrdinalIgnoreCase),
        };
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

        // Für frühe Versionen reicht eine robuste Volltextsuche über die
        // wichtigsten Felder. Strukturierte Filter werden separat vor dieser
        // Volltextsuche angewendet, sodass beides kombiniert werden kann.
        return Contains(entry.Title, searchText)
            || Contains(entry.UserName, searchText)
            || Contains(entry.Notes, searchText)
            || Contains(groupPath, searchText)
            || entry.Tags.Any(tag => Contains(tag, searchText))
            || entry.CustomFields.Any(field => Contains(field.Name, searchText) || Contains(field.Value, searchText));
    }

    private static bool HasCustomFieldKindOrName(SecretEntry entry, CustomFieldKind expectedKind, params string[] nameFragments)
    {
        return entry.CustomFields.Any(field => field.Kind == expectedKind || ContainsAnyFragment(field.Name, nameFragments));
    }

    private static bool ContainsAnyFragment(string? value, IEnumerable<string> fragments)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return fragments.Any(fragment => value.Contains(fragment, StringComparison.OrdinalIgnoreCase));
    }

    private static bool Contains(string? value, string searchText)
    {
        return value?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true;
    }
}
