using Sasd.SecretManager.Domain;

// ============================================================================
// Dateiüberblick:
// Hält UI-nahe Regeln, die bewusst testbar in der Application-Schicht liegen, etwa Root-Verhalten und Bestätigungstexte.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.Application;

/// <summary>
/// Bündelt kleine UX-Regeln für die Arbeitsoberfläche des Tresors.
/// Die Regeln bleiben bewusst fachlich simpel und dadurch gut testbar.
/// </summary>
public sealed class VaultUxPolicyService
{
    /// <summary>
    /// Entscheidet, ob die Oberfläche den ersten sichtbaren Eintrag automatisch auswählen soll.
    /// Auf der Root-Ebene ohne aktive Suche wird bewusst keine automatische Eintragsauswahl erzwungen,
    /// damit die Tresorübersicht ruhiger und verständlicher bleibt.
    /// </summary>
    /// <summary>
    /// Entscheidet, ob die UI nach einem Kontextwechsel automatisch den ersten Eintrag selektieren soll.
    /// </summary>
    public bool ShouldAutoSelectFirstEntry(string? selectedGroupPath, string? searchText, int visibleEntryCount)
    {
        if (visibleEntryCount <= 0)
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(selectedGroupPath)
            || !string.IsNullOrWhiteSpace(searchText);
    }

    /// <summary>
    /// Erstellt den Text für die Statusleiste passend zum aktuellen Auswahlkontext.
    /// </summary>
    /// <summary>
    /// Liefert den passenden Statuszeilentext für Root- oder Gruppenansichten.
    /// </summary>
    public string BuildSelectionStatusText(
        SecretVault vault,
        string? selectedGroupPath,
        int visibleEntryCount,
        int sortColumn,
        bool sortAscending,
        string? searchText,
        string? filePath,
        bool isDirty)
    {
        var fileLabel = string.IsNullOrWhiteSpace(filePath)
            ? " · ohne Datei"
            : $" · Datei: {Path.GetFileName(filePath)}";
        var dirtyLabel = isDirty ? " · ungespeichert" : string.Empty;

        if (string.IsNullOrWhiteSpace(selectedGroupPath) && string.IsNullOrWhiteSpace(searchText))
        {
            return $"Tresorübersicht · {vault.Groups.Count} Gruppen · {visibleEntryCount} Einträge{fileLabel}{dirtyLabel}";
        }

        var groupLabel = string.IsNullOrWhiteSpace(selectedGroupPath) ? "Alle Gruppen" : selectedGroupPath;
        var searchLabel = string.IsNullOrWhiteSpace(searchText) ? string.Empty : $" · Suche: {searchText}";
        var sortDirection = sortAscending ? "aufsteigend" : "absteigend";
        return $"{groupLabel} · {visibleEntryCount} Einträge · Sortierung Spalte {sortColumn + 1} ({sortDirection}){searchLabel}{fileLabel}{dirtyLabel}";
    }

    /// <summary>
    /// Ermittelt, wie groß die fachlichen Auswirkungen einer Gruppenverschiebung sind.
    /// </summary>
    /// <summary>
    /// Berechnet, welche Untergruppen und Einträge von einer Gruppenverschiebung betroffen sind.
    /// </summary>
    public GroupMoveImpact GetGroupMoveImpact(SecretVault vault, string sourceGroupPath)
    {
        var sourceGroup = vault.Groups.FirstOrDefault(group => string.Equals(group.Path, sourceGroupPath, StringComparison.OrdinalIgnoreCase));
        if (sourceGroup is null)
        {
            return new GroupMoveImpact(0, 0);
        }

        var subtreeGroups = vault.Groups
            .Where(group => string.Equals(group.Path, sourceGroupPath, StringComparison.OrdinalIgnoreCase)
                || group.Path.StartsWith(sourceGroupPath + "/", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var groupIds = subtreeGroups.Select(group => group.Id).ToHashSet();
        var entryCount = vault.Entries.Count(entry => entry.GroupId is Guid groupId && groupIds.Contains(groupId));
        var childGroupCount = Math.Max(0, subtreeGroups.Count - 1);

        return new GroupMoveImpact(childGroupCount, entryCount);
    }

    /// <summary>
    /// Kennzeichnet, ob vor dem Verschieben einer Gruppe eine Rückfrage sinnvoll ist.
    /// </summary>
    public bool ShouldConfirmGroupMove(int childGroupCount, int entryCount)
        => childGroupCount > 0 || entryCount > 0;

    /// <summary>
    /// Baut einen verständlichen Rückfragetext für das Verschieben einer Gruppe.
    /// </summary>
    public string BuildGroupMoveConfirmationMessage(string sourceGroupPath, string targetLabel, int childGroupCount, int entryCount)
    {
        var impactParts = new List<string>();
        if (childGroupCount > 0)
        {
            impactParts.Add($"{childGroupCount} Untergruppe{(childGroupCount == 1 ? string.Empty : "n")}");
        }

        if (entryCount > 0)
        {
            impactParts.Add($"{entryCount} Eintrag{(entryCount == 1 ? string.Empty : "e")}");
        }

        var impactText = impactParts.Count == 0
            ? "keine weiteren Inhalte"
            : string.Join(" und ", impactParts);

        return $"Möchtest du die Gruppe '{sourceGroupPath}' wirklich nach '{targetLabel}' verschieben?{Environment.NewLine}{Environment.NewLine}Betroffen: {impactText}.";
    }
}

/// <summary>
/// Beschreibt die fachlichen Auswirkungen einer Gruppenverschiebung.
/// </summary>
public readonly record struct GroupMoveImpact(int ChildGroupCount, int EntryCount);
