using Sasd.SecretManager.Domain;
using Xunit;

namespace Sasd.SecretManager.Application.Tests;

/// <summary>
/// Tests für die fachliche Eintragsvalidierung aus DSM-003.
/// </summary>
/// <remarks>
/// Die Tests konzentrieren sich bewusst auf Regeln, die vor späterem Import,
/// Export und produktiver Nutzung wichtig sind: Pflichtfelder, Gruppenbezug,
/// Eindeutigkeit, Zusatzfeldsyntax und einfache Typprüfung.
/// </remarks>
public sealed class EntryValidationServiceTests
{
    private readonly EntryValidationService _service = new();

    [Fact]
    public void ValidateStandalone_AcceptsMinimalValidEntry()
    {
        var model = new EntryEditModel
        {
            Title = "IONOS Kundencenter",
            UserName = "admin",
            Secret = "secret",
            CustomFieldsText = "URL = https://example.org",
        };

        var result = _service.ValidateStandalone(model);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateStandalone_RejectsEmptyTitle()
    {
        var model = new EntryEditModel
        {
            Title = "   ",
        };

        var result = _service.ValidateStandalone(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, issue => issue.PropertyName == nameof(EntryEditModel.Title));
    }

    [Fact]
    public void ValidateForCreate_RejectsUnknownGroup()
    {
        var vault = CreateVaultWithGroups();
        var model = new EntryEditModel
        {
            Title = "Server Root",
            SelectedGroupPath = "Nicht vorhanden",
        };

        var result = _service.ValidateForCreate(vault, model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, issue => issue.PropertyName == nameof(EntryEditModel.SelectedGroupPath));
    }

    [Fact]
    public void ValidateForCreate_RejectsDuplicateTitleInsideSameGroup()
    {
        var vault = CreateVaultWithGroups();
        var group = vault.Groups.Single(item => item.Path == "Server");
        vault.Entries.Add(new SecretEntry
        {
            Title = "Root Login",
            GroupId = group.Id,
        });

        var model = new EntryEditModel
        {
            Title = "root login",
            SelectedGroupPath = "Server",
        };

        var result = _service.ValidateForCreate(vault, model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, issue => issue.Message.Contains("bereits", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateForCreate_AllowsSameTitleInDifferentGroup()
    {
        var vault = CreateVaultWithGroups();
        var serverGroup = vault.Groups.Single(item => item.Path == "Server");
        vault.Entries.Add(new SecretEntry
        {
            Title = "Root Login",
            GroupId = serverGroup.Id,
        });

        var model = new EntryEditModel
        {
            Title = "Root Login",
            SelectedGroupPath = "Datenbanken",
        };

        var result = _service.ValidateForCreate(vault, model);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateForUpdate_AllowsCurrentEntryToKeepItsTitle()
    {
        var vault = CreateVaultWithGroups();
        var group = vault.Groups.Single(item => item.Path == "Server");
        var entry = new SecretEntry
        {
            Title = "Root Login",
            GroupId = group.Id,
        };
        vault.Entries.Add(entry);

        var model = new EntryEditModel
        {
            Title = "Root Login",
            SelectedGroupPath = "Server",
        };

        var result = _service.ValidateForUpdate(vault, entry, model);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateStandalone_RejectsCustomFieldWithoutSeparator()
    {
        var model = new EntryEditModel
        {
            Title = "Eintrag",
            CustomFieldsText = "Host db.example.org",
        };

        var result = _service.ValidateStandalone(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, issue => issue.PropertyName == nameof(EntryEditModel.CustomFieldsText));
    }

    [Fact]
    public void ValidateStandalone_RejectsDuplicateCustomFieldNames()
    {
        var model = new EntryEditModel
        {
            Title = "Eintrag",
            CustomFieldsText = "Host = db1\nHost = db2",
        };

        var result = _service.ValidateStandalone(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, issue => issue.Message.Contains("mehrfach", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateStandalone_RejectsInvalidPortNumber()
    {
        var model = new EntryEditModel
        {
            Title = "Datenbank",
            CustomFieldsText = "Port = 70000",
        };

        var result = _service.ValidateStandalone(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, issue => issue.Message.Contains("Portnummer", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateStandalone_AcceptsValidPortNumber()
    {
        var model = new EntryEditModel
        {
            Title = "Datenbank",
            CustomFieldsText = "Port = 3306",
        };

        var result = _service.ValidateStandalone(model);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateStandalone_RejectsInvalidHttpUrl()
    {
        var model = new EntryEditModel
        {
            Title = "Webseite",
            CustomFieldsText = "URL = https://",
        };

        var result = _service.ValidateStandalone(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, issue => issue.Message.Contains("URL", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateForCreate_ThrowsEntryValidationException_WhenInvalid()
    {
        var vault = CreateVaultWithGroups();
        var model = new EntryEditModel
        {
            Title = string.Empty,
        };

        var exception = Assert.Throws<EntryValidationException>(() => _service.ValidateForCreate(vault, model).ThrowIfInvalid());

        Assert.NotEmpty(exception.Issues);
    }

    private static SecretVault CreateVaultWithGroups()
    {
        return new SecretVault
        {
            Name = "Test Vault",
            Groups =
            {
                new EntryGroup
                {
                    Name = "Server",
                    Path = "Server",
                },
                new EntryGroup
                {
                    Name = "Datenbanken",
                    Path = "Datenbanken",
                },
            },
        };
    }
}
