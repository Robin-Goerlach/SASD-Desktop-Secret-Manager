using Sasd.SecretManager.Application;
using Sasd.SecretManager.Domain;
using Xunit;

// ============================================================================
// Dateiüberblick:
// Enthält Unit-Tests zur Verifikation des beschriebenen Fach- und UI-nahen Verhaltens.
// Die Testnamen sind sprechend gewählt und dienen zugleich als Dokumentation
// des erwarteten Verhaltens der produktiven Klassen.
// ============================================================================

namespace Sasd.SecretManager.Application.Tests;

/// <summary>
    /// Testklasse für VaultUxPolicyService und das dazugehörige erwartete Verhalten.
    /// </summary>
public sealed class VaultUxPolicyServiceTests
{
    private readonly VaultUxPolicyService _service = new();/// <summary>
    /// Verifiziert: ShouldAutoSelectFirstEntry ReturnsFalse OnRootWithoutSearch.
    /// </summary>
    [Fact]
    public void ShouldAutoSelectFirstEntry_ReturnsFalse_OnRootWithoutSearch()
    {
        var result = _service.ShouldAutoSelectFirstEntry(null, string.Empty, 5);
        Assert.False(result);
    }

    /// <summary>
    /// Verifiziert: ShouldAutoSelectFirstEntry ReturnsTrue OnRootWithSearch.
    /// </summary>
    [Fact]
    public void ShouldAutoSelectFirstEntry_ReturnsTrue_OnRootWithSearch()
    {
        var result = _service.ShouldAutoSelectFirstEntry(null, "GitHub", 2);
        Assert.True(result);
    }

    /// <summary>
    /// Verifiziert: ShouldAutoSelectFirstEntry ReturnsTrue OnConcreteGroup.
    /// </summary>
    [Fact]
    public void ShouldAutoSelectFirstEntry_ReturnsTrue_OnConcreteGroup()
    {
        var result = _service.ShouldAutoSelectFirstEntry("Privat", string.Empty, 2);
        Assert.True(result);
    }

    /// <summary>
    /// Verifiziert: GetGroupMoveImpact CountsDescendantsAndEntriesInSubtree.
    /// </summary>
    [Fact]
    public void GetGroupMoveImpact_CountsDescendantsAndEntriesInSubtree()
    {
        var vault = CreateVault();

        var impact = _service.GetGroupMoveImpact(vault, "SASD-GmbH");

        Assert.Equal(2, impact.ChildGroupCount);
        Assert.Equal(2, impact.EntryCount);
    }

    /// <summary>
    /// Verifiziert: BuildSelectionStatusText UsesOverviewTextOnRootWithoutSearch.
    /// </summary>
    [Fact]
    public void BuildSelectionStatusText_UsesOverviewTextOnRootWithoutSearch()
    {
        var vault = CreateVault();

        var text = _service.BuildSelectionStatusText(vault, null, 3, 0, true, string.Empty, null, false);

        Assert.Contains("Tresorübersicht", text);
        Assert.Contains("3 Gruppen", text);
        Assert.Contains("3 Einträge", text);
    }

    private static SecretVault CreateVault()
    {
        var root = new EntryGroup { Name = "SASD-GmbH", Path = "SASD-GmbH" };
        var childA = new EntryGroup { Name = "GitHub", ParentGroupId = root.Id, Path = "SASD-GmbH/GitHub" };
        var childB = new EntryGroup { Name = "IONOS", ParentGroupId = root.Id, Path = "SASD-GmbH/IONOS" };

        var vault = new SecretVault { Name = "Test Vault" };
        vault.Groups.Add(root);
        vault.Groups.Add(childA);
        vault.Groups.Add(childB);
        vault.Entries.Add(new SecretEntry { Title = "GitHub Organisation", GroupId = childA.Id });
        vault.Entries.Add(new SecretEntry { Title = "IONOS Webspace FTP", GroupId = childB.Id });
        vault.Entries.Add(new SecretEntry { Title = "Freier Eintrag" });
        return vault;
    }
}
