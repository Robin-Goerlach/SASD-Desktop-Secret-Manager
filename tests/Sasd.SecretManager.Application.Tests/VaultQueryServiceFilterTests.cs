using Sasd.SecretManager.Domain;
using Xunit;

namespace Sasd.SecretManager.Application.Tests;

/// <summary>
/// Tests für DSM-004: strukturierte Suche und Filter in der Eintragsliste.
/// </summary>
/// <remarks>
/// Die Tests prüfen bewusst die Application-Schicht und nicht die WinForms-
/// Oberfläche. Damit bleibt die Filterlogik unabhängig von konkreten Controls
/// und kann später auch für Importberichte oder andere Oberflächen genutzt
/// werden.
/// </remarks>
public sealed class VaultQueryServiceFilterTests
{
    private readonly VaultQueryService _service = new();

    [Fact]
    public void GetVisibleEntries_FiltersByEntryType()
    {
        var vault = CreateVault();

        var result = _service.GetVisibleEntries(vault, new EntryFilterCriteria
        {
            EntryType = EntryType.ApiKey,
            SortColumn = 0,
            SortAscending = true,
        });

        var entry = Assert.Single(result);
        Assert.Equal("OpenAI API Token", entry.Title);
    }

    [Fact]
    public void GetVisibleEntries_FiltersByTagCaseInsensitive()
    {
        var vault = CreateVault();

        var result = _service.GetVisibleEntries(vault, new EntryFilterCriteria
        {
            Tag = "PROD",
            SortColumn = 0,
            SortAscending = true,
        });

        Assert.Equal(2, result.Count);
        Assert.All(result, entry => Assert.Contains(entry.Tags, tag => string.Equals(tag, "prod", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void GetVisibleEntries_FiltersEntriesWithoutGroup()
    {
        var vault = CreateVault();

        var result = _service.GetVisibleEntries(vault, new EntryFilterCriteria
        {
            SpecialFilter = EntrySpecialFilter.WithoutGroup,
        });

        // Der Spezialfilter "Ohne Gruppe" soll fachlich alle Einträge liefern,
        // deren GroupId leer ist. Das betrifft im Test-Tresor bewusst zwei
        // Einträge: einen API-Key und eine unsortierte Notiz. Die erste Version
        // dieses Tests erwartete versehentlich nur die Notiz und war damit
        // strenger als die eigentliche Filteranforderung.
        Assert.Equal(2, result.Count);
        Assert.All(result, entry => Assert.Null(entry.GroupId));
        Assert.Contains(result, entry => entry.Title == "OpenAI API Token");
        Assert.Contains(result, entry => entry.Title == "Unsortierte Notiz");
    }

    [Fact]
    public void GetVisibleEntries_FiltersEntriesWithUrlField()
    {
        var vault = CreateVault();

        var result = _service.GetVisibleEntries(vault, new EntryFilterCriteria
        {
            SpecialFilter = EntrySpecialFilter.WithUrlField,
        });

        var entry = Assert.Single(result);
        Assert.Equal("IONOS Login", entry.Title);
    }

    [Fact]
    public void GetVisibleEntries_FiltersEntriesWithHostField()
    {
        var vault = CreateVault();

        var result = _service.GetVisibleEntries(vault, new EntryFilterCriteria
        {
            SpecialFilter = EntrySpecialFilter.WithHostField,
        });

        Assert.Equal("MariaDB Root", Assert.Single(result).Title);
    }

    [Fact]
    public void GetVisibleEntries_FiltersEntriesWithEmailField()
    {
        var vault = CreateVault();

        var result = _service.GetVisibleEntries(vault, new EntryFilterCriteria
        {
            SpecialFilter = EntrySpecialFilter.WithEmailField,
        });

        Assert.Equal("Mailkonto", Assert.Single(result).Title);
    }

    [Fact]
    public void GetVisibleEntries_CombinesGroupTypeTagAndSearch()
    {
        var vault = CreateVault();

        var result = _service.GetVisibleEntries(vault, new EntryFilterCriteria
        {
            SelectedGroupPath = "Server",
            EntryType = EntryType.Database,
            Tag = "prod",
            SearchText = "MariaDB",
            SortColumn = 0,
            SortAscending = true,
        });

        Assert.Equal("MariaDB Root", Assert.Single(result).Title);
    }

    [Fact]
    public void GetVisibleEntries_KeepsOriginalOverloadCompatible()
    {
        var vault = CreateVault();

        var result = _service.GetVisibleEntries(vault, "Server", "mariadb", 0, true);

        Assert.Equal("MariaDB Root", Assert.Single(result).Title);
    }

    private static SecretVault CreateVault()
    {
        var serverGroup = new EntryGroup
        {
            Name = "Server",
            Path = "Server",
        };

        var mailGroup = new EntryGroup
        {
            Name = "Mail",
            Path = "Mail",
        };

        var vault = new SecretVault
        {
            Name = "Filter Test Vault",
            Groups =
            {
                serverGroup,
                mailGroup,
            },
            KnownTags =
            {
                "prod",
                "test",
                "cloud",
            },
        };

        vault.Entries.Add(new SecretEntry
        {
            Title = "IONOS Login",
            EntryType = EntryType.Login,
            UserName = "admin",
            GroupId = serverGroup.Id,
            Tags = { "prod", "hosting" },
            CustomFields =
            {
                new CustomField
                {
                    Name = "URL",
                    Kind = CustomFieldKind.Url,
                    Value = "https://login.ionos.de",
                },
            },
        });

        vault.Entries.Add(new SecretEntry
        {
            Title = "MariaDB Root",
            EntryType = EntryType.Database,
            UserName = "root",
            GroupId = serverGroup.Id,
            Tags = { "prod", "database" },
            CustomFields =
            {
                new CustomField
                {
                    Name = "Host",
                    Kind = CustomFieldKind.Hostname,
                    Value = "db.example.test",
                },
            },
        });

        vault.Entries.Add(new SecretEntry
        {
            Title = "OpenAI API Token",
            EntryType = EntryType.ApiKey,
            UserName = "service-account",
            Tags = { "cloud" },
            CustomFields =
            {
                new CustomField
                {
                    Name = "API Secret",
                    Kind = CustomFieldKind.Secret,
                    Value = "secret",
                    IsSecret = true,
                },
            },
        });

        vault.Entries.Add(new SecretEntry
        {
            Title = "Mailkonto",
            EntryType = EntryType.Mail,
            UserName = "info@example.test",
            GroupId = mailGroup.Id,
            Tags = { "mail" },
            CustomFields =
            {
                new CustomField
                {
                    Name = "E-Mail",
                    Kind = CustomFieldKind.Email,
                    Value = "info@example.test",
                },
            },
        });

        vault.Entries.Add(new SecretEntry
        {
            Title = "Unsortierte Notiz",
            EntryType = EntryType.SecureNote,
            Notes = "Noch keiner Gruppe zugeordnet.",
        });

        return vault;
    }
}
