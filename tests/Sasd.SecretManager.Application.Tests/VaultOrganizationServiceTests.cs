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
    /// Testklasse für VaultOrganizationService und das dazugehörige erwartete Verhalten.
    /// </summary>
public sealed class VaultOrganizationServiceTests
{
    private readonly VaultOrganizationService _service = new();/// <summary>
    /// Verifiziert: CreateGroup CreatesNestedPath WhenParentIsGiven.
    /// </summary>
    [Fact]
    public void CreateGroup_CreatesNestedPath_WhenParentIsGiven()
    {
        var vault = CreateVault();

        var group = _service.CreateGroup(vault, "API", "SASD-GmbH/IONOS");

        Assert.Equal("API", group.Name);
        Assert.Equal("SASD-GmbH/IONOS/API", group.Path);
        Assert.NotNull(group.ParentGroupId);
    }

    /// <summary>
    /// Verifiziert: RenameGroup UpdatesDescendantPaths.
    /// </summary>
    [Fact]
    public void RenameGroup_UpdatesDescendantPaths()
    {
        var vault = CreateVault();

        var newPath = _service.RenameGroup(vault, "SASD-GmbH/IONOS", "Hosting");

        Assert.Equal("SASD-GmbH/Hosting", newPath);
        Assert.Contains(vault.Groups, group => group.Path == "SASD-GmbH/Hosting/Mail");
    }

    /// <summary>
    /// Verifiziert: MoveGroup ToAnotherParent UpdatesDescendants.
    /// </summary>
    [Fact]
    public void MoveGroup_ToAnotherParent_UpdatesDescendants()
    {
        var vault = CreateVault();

        var newPath = _service.MoveGroup(vault, "SASD-GmbH/IONOS", "Privat");

        Assert.Equal("Privat/IONOS", newPath);
        Assert.Contains(vault.Groups, group => group.Path == "Privat/IONOS/Mail");
        Assert.Equal(
            vault.Groups.Single(group => group.Path == "Privat").Id,
            vault.Groups.Single(group => group.Path == "Privat/IONOS").ParentGroupId);
    }

    /// <summary>
    /// Verifiziert: MoveGroup ToRoot ClearsParentGroupId.
    /// </summary>
    [Fact]
    public void MoveGroup_ToRoot_ClearsParentGroupId()
    {
        var vault = CreateVault();

        var newPath = _service.MoveGroup(vault, "SASD-GmbH/IONOS", null);

        Assert.Equal("IONOS", newPath);
        var movedGroup = vault.Groups.Single(group => group.Path == "IONOS");
        Assert.Null(movedGroup.ParentGroupId);
        Assert.Contains(vault.Groups, group => group.Path == "IONOS/Mail");
    }

    /// <summary>
    /// Verifiziert: MoveGroup Throws WhenTargetIsDescendant.
    /// </summary>
    [Fact]
    public void MoveGroup_Throws_WhenTargetIsDescendant()
    {
        var vault = CreateVault();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            _service.MoveGroup(vault, "SASD-GmbH/IONOS", "SASD-GmbH/IONOS/Mail"));

        Assert.Contains("Untergruppe", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifiziert: MoveGroup ReturnsNull WhenPositionDoesNotChange.
    /// </summary>
    [Fact]
    public void MoveGroup_ReturnsNull_WhenPositionDoesNotChange()
    {
        var vault = CreateVault();

        var newPath = _service.MoveGroup(vault, "SASD-GmbH/IONOS", "SASD-GmbH");

        Assert.Null(newPath);
    }

    /// <summary>
    /// Verifiziert: DeleteGroup Throws WhenEntriesStillExist.
    /// </summary>
    [Fact]
    public void DeleteGroup_Throws_WhenEntriesStillExist()
    {
        var vault = CreateVault();

        var exception = Assert.Throws<InvalidOperationException>(() => _service.DeleteGroup(vault, "SASD-GmbH/IONOS/Mail"));

        Assert.Contains("Einträge", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifiziert: MoveEntryToGroup ChangesGroupId.
    /// </summary>
    [Fact]
    public void MoveEntryToGroup_ChangesGroupId()
    {
        var vault = CreateVault();
        var entry = vault.Entries[0];

        var archive = _service.CreateGroup(vault, "Archiv", "SASD-GmbH");
        var changed = _service.MoveEntryToGroup(vault, entry, archive.Path);

        Assert.True(changed);
        Assert.Equal(archive.Id, entry.GroupId);
    }

    /// <summary>
    /// Verifiziert: MoveEntryToGroup ReturnsFalse WhenEntryAlreadyInTargetGroup.
    /// </summary>
    [Fact]
    public void MoveEntryToGroup_ReturnsFalse_WhenEntryAlreadyInTargetGroup()
    {
        var vault = CreateVault();
        var entry = vault.Entries[0];

        var changed = _service.MoveEntryToGroup(vault, entry, "SASD-GmbH/IONOS/Mail");

        Assert.False(changed);
    }

    /// <summary>
    /// Verifiziert: DeleteEntry RemovesEntryFromVault.
    /// </summary>
    [Fact]
    public void DeleteEntry_RemovesEntryFromVault()
    {
        var vault = CreateVault();
        var entry = vault.Entries[0];

        var removed = _service.DeleteEntry(vault, entry);

        Assert.True(removed);
        Assert.DoesNotContain(entry, vault.Entries);
    }

    private static SecretVault CreateVault()
    {
        var sasd = new EntryGroup { Name = "SASD-GmbH", Path = "SASD-GmbH" };
        var privat = new EntryGroup { Name = "Privat", Path = "Privat" };
        var ionos = new EntryGroup { Name = "IONOS", ParentGroupId = sasd.Id, Path = "SASD-GmbH/IONOS" };
        var mail = new EntryGroup { Name = "Mail", ParentGroupId = ionos.Id, Path = "SASD-GmbH/IONOS/Mail" };

        var vault = new SecretVault { Name = "Test Vault" };
        vault.Groups.AddRange([sasd, privat, ionos, mail]);
        vault.Entries.Add(new SecretEntry
        {
            Title = "Support Mail",
            EntryType = EntryType.Mail,
            UserName = "support@example.invalid",
            Secret = "secret",
            GroupId = mail.Id,
        });

        return vault;
    }
}
