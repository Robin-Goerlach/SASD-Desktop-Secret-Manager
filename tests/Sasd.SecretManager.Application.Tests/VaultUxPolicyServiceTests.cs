using Sasd.SecretManager.Application;
using Sasd.SecretManager.Domain;
using Xunit;

namespace Sasd.SecretManager.Application.Tests;

public sealed class VaultUxPolicyServiceTests
{
    private readonly VaultUxPolicyService _service = new();

    [Fact]
    public void ShouldAutoSelectFirstEntry_ReturnsFalse_OnRootWithoutSearch()
    {
        var result = _service.ShouldAutoSelectFirstEntry(null, string.Empty, 5);
        Assert.False(result);
    }

    [Fact]
    public void ShouldAutoSelectFirstEntry_ReturnsTrue_OnRootWithSearch()
    {
        var result = _service.ShouldAutoSelectFirstEntry(null, "GitHub", 2);
        Assert.True(result);
    }

    [Fact]
    public void ShouldAutoSelectFirstEntry_ReturnsTrue_OnConcreteGroup()
    {
        var result = _service.ShouldAutoSelectFirstEntry("Privat", string.Empty, 2);
        Assert.True(result);
    }

    [Fact]
    public void GetGroupMoveImpact_CountsDescendantsAndEntriesInSubtree()
    {
        var vault = CreateVault();

        var impact = _service.GetGroupMoveImpact(vault, "SASD-GmbH");

        Assert.Equal(2, impact.ChildGroupCount);
        Assert.Equal(2, impact.EntryCount);
    }

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
