using Sasd.SecretManager.Application;
using Sasd.SecretManager.Domain;
using Xunit;

namespace Sasd.SecretManager.Application.Tests;

public sealed class VaultOrganizationServiceTests
{
    private readonly VaultOrganizationService _service = new();

    [Fact]
    public void CreateGroup_CreatesNestedPath_WhenParentIsGiven()
    {
        var vault = CreateVault();

        var group = _service.CreateGroup(vault, "API", "SASD-GmbH/IONOS");

        Assert.Equal("API", group.Name);
        Assert.Equal("SASD-GmbH/IONOS/API", group.Path);
        Assert.NotNull(group.ParentGroupId);
    }

    [Fact]
    public void RenameGroup_UpdatesDescendantPaths()
    {
        var vault = CreateVault();

        var newPath = _service.RenameGroup(vault, "SASD-GmbH/IONOS", "Hosting");

        Assert.Equal("SASD-GmbH/Hosting", newPath);
        Assert.Contains(vault.Groups, group => group.Path == "SASD-GmbH/Hosting/Mail");
    }

    [Fact]
    public void DeleteGroup_Throws_WhenEntriesStillExist()
    {
        var vault = CreateVault();

        var exception = Assert.Throws<InvalidOperationException>(() => _service.DeleteGroup(vault, "SASD-GmbH/IONOS/Mail"));

        Assert.Contains("Einträge", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MoveEntryToGroup_ChangesGroupId()
    {
        var vault = CreateVault();
        var entry = vault.Entries[0];

        var targetGroup = _service.CreateGroup(vault, "API", "SASD-GmbH/IONOS");

        var changed = _service.MoveEntryToGroup(vault, entry, targetGroup.Path);

        Assert.True(changed);
        Assert.Equal(targetGroup.Id, entry.GroupId);
    }

    [Fact]
    public void MoveEntryToGroup_ReturnsFalse_WhenEntryAlreadyInTargetGroup()
    {
        var vault = CreateVault();
        var entry = vault.Entries[0];

        var changed = _service.MoveEntryToGroup(vault, entry, "SASD-GmbH/IONOS/Mail");

        Assert.False(changed);
    }

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
        var root = new EntryGroup { Name = "SASD-GmbH", Path = "SASD-GmbH" };
        var ionos = new EntryGroup { Name = "IONOS", ParentGroupId = root.Id, Path = "SASD-GmbH/IONOS" };
        var mail = new EntryGroup { Name = "Mail", ParentGroupId = ionos.Id, Path = "SASD-GmbH/IONOS/Mail" };

        var vault = new SecretVault { Name = "Test Vault" };
        vault.Groups.AddRange([root, ionos, mail]);
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
