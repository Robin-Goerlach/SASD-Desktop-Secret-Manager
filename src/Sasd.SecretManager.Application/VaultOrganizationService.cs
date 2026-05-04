using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Kapselt Organisationsoperationen im In-Memory-Tresor,
/// insbesondere Gruppenpflege, Eintragslöschung und Gruppenzuordnung.
/// </summary>
public sealed class VaultOrganizationService
{
    /// <summary>
    /// Legt eine neue Gruppe an. Ist ein Elternpfad angegeben,
    /// wird die Gruppe als Untergruppe darunter angelegt.
    /// </summary>
    public EntryGroup CreateGroup(SecretVault vault, string name, string? parentGroupPath)
    {
        ArgumentNullException.ThrowIfNull(vault);

        var normalizedName = NormalizeName(name);
        var parentGroup = ResolveGroup(vault, parentGroupPath, allowNull: true);
        var newPath = parentGroup is null ? normalizedName : $"{parentGroup.Path}/{normalizedName}";

        EnsurePathIsUnique(vault, newPath, excludingGroupId: null);

        var group = new EntryGroup
        {
            Name = normalizedName,
            ParentGroupId = parentGroup?.Id,
            Path = newPath,
        };

        vault.Groups.Add(group);
        return group;
    }

    /// <summary>
    /// Benennt eine vorhandene Gruppe um und aktualisiert dabei
    /// alle untergeordneten Pfade.
    /// </summary>
    public string RenameGroup(SecretVault vault, string currentGroupPath, string newName)
    {
        ArgumentNullException.ThrowIfNull(vault);

        var group = ResolveGroup(vault, currentGroupPath, allowNull: false)!;
        var normalizedName = NormalizeName(newName);
        var parentGroup = group.ParentGroupId is Guid parentId
            ? vault.Groups.FirstOrDefault(item => item.Id == parentId)
            : null;

        var newPath = parentGroup is null ? normalizedName : $"{parentGroup.Path}/{normalizedName}";
        EnsurePathIsUnique(vault, newPath, excludingGroupId: group.Id);

        var oldPath = group.Path;
        group.Name = normalizedName;
        group.Path = newPath;

        foreach (var descendant in GetDescendants(vault, group.Id))
        {
            descendant.Path = newPath + descendant.Path[oldPath.Length..];
        }

        return newPath;
    }

    /// <summary>
    /// Löscht eine Gruppe nur dann, wenn weder Untergruppen
    /// noch direkt zugewiesene Einträge vorhanden sind.
    /// </summary>
    public void DeleteGroup(SecretVault vault, string groupPath)
    {
        ArgumentNullException.ThrowIfNull(vault);

        var group = ResolveGroup(vault, groupPath, allowNull: false)!;

        if (vault.Groups.Any(item => item.ParentGroupId == group.Id))
        {
            throw new InvalidOperationException("Die Gruppe enthält Untergruppen und kann deshalb nicht gelöscht werden.");
        }

        if (vault.Entries.Any(item => item.GroupId == group.Id))
        {
            throw new InvalidOperationException("Die Gruppe enthält noch Einträge und kann deshalb nicht gelöscht werden.");
        }

        vault.Groups.Remove(group);
    }

    /// <summary>
    /// Verschiebt einen Eintrag in die Zielgruppe.
    /// Liefert <c>true</c>, wenn sich die Gruppenzuordnung geändert hat.
    /// </summary>
    public bool MoveEntryToGroup(SecretVault vault, SecretEntry entry, string targetGroupPath)
    {
        ArgumentNullException.ThrowIfNull(vault);
        ArgumentNullException.ThrowIfNull(entry);

        var targetGroup = ResolveGroup(vault, targetGroupPath, allowNull: false)!;
        if (entry.GroupId == targetGroup.Id)
        {
            return false;
        }

        entry.GroupId = targetGroup.Id;
        entry.ModifiedUtc = DateTimeOffset.UtcNow;
        return true;
    }

    /// <summary>
    /// Entfernt einen Eintrag aus dem Tresor.
    /// </summary>
    public bool DeleteEntry(SecretVault vault, SecretEntry entry)
    {
        ArgumentNullException.ThrowIfNull(vault);
        ArgumentNullException.ThrowIfNull(entry);

        return vault.Entries.Remove(entry);
    }

    private static EntryGroup? ResolveGroup(SecretVault vault, string? groupPath, bool allowNull)
    {
        if (string.IsNullOrWhiteSpace(groupPath))
        {
            if (allowNull)
            {
                return null;
            }

            throw new InvalidOperationException("Bitte zuerst eine Gruppe auswählen.");
        }

        var group = vault.Groups.FirstOrDefault(item => string.Equals(item.Path, groupPath, StringComparison.OrdinalIgnoreCase));
        if (group is null && !allowNull)
        {
            throw new InvalidOperationException("Die angegebene Gruppe konnte nicht gefunden werden.");
        }

        return group;
    }

    private static IEnumerable<EntryGroup> GetDescendants(SecretVault vault, Guid groupId)
    {
        var groupsByParent = vault.Groups.ToLookup(item => item.ParentGroupId);
        var queue = new Queue<EntryGroup>(groupsByParent[groupId]);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return current;

            foreach (var child in groupsByParent[current.Id])
            {
                queue.Enqueue(child);
            }
        }
    }

    private static void EnsurePathIsUnique(SecretVault vault, string path, Guid? excludingGroupId)
    {
        var duplicateExists = vault.Groups.Any(item =>
            item.Id != excludingGroupId
            && string.Equals(item.Path, path, StringComparison.OrdinalIgnoreCase));

        if (duplicateExists)
        {
            throw new InvalidOperationException("Es existiert bereits eine Gruppe mit demselben Pfad.");
        }
    }

    private static string NormalizeName(string? name)
    {
        var normalizedName = (name ?? string.Empty).Trim();
        if (normalizedName.Length == 0)
        {
            throw new InvalidOperationException("Bitte einen Gruppennamen angeben.");
        }

        if (normalizedName.Contains('/'))
        {
            throw new InvalidOperationException("Gruppennamen dürfen kein '/' enthalten.");
        }

        return normalizedName;
    }
}
