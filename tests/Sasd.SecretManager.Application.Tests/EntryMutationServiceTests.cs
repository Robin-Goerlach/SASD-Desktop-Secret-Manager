using Sasd.SecretManager.Application;
using Sasd.SecretManager.Domain;
using Xunit;

namespace Sasd.SecretManager.Application.Tests;

public sealed class EntryMutationServiceTests
{
    [Fact]
    public void CreateEntry_AddsEntryTagsAndCustomFields()
    {
        var vault = new DemoVaultFactory().CreateDemoVault();
        var service = new EntryMutationService();
        var model = new EntryEditModel
        {
            Title = "Neuer API Zugang",
            EntryType = EntryType.ApiKey,
            UserName = "api-user",
            Secret = "Api-Example!2026#Token",
            SelectedGroupPath = "SASD-GmbH/GitHub",
            TagsText = "SASD, API, GitHub",
            Notes = "Testeintrag",
            CustomFieldsText = "URL = https://api.example.invalid\n!Client Secret = secret-123",
        };

        var entry = service.CreateEntry(vault, model);

        Assert.Contains(entry, vault.Entries);
        Assert.Equal(EntryType.ApiKey, entry.EntryType);
        Assert.Equal("api-user", entry.UserName);
        Assert.Contains("API", entry.Tags);
        Assert.Equal(2, entry.CustomFields.Count);
        Assert.True(entry.CustomFields[1].IsSecret);
    }

    [Fact]
    public void UpdateEntry_ReplacesTagsAndChangesModifiedTimestamp()
    {
        var vault = new DemoVaultFactory().CreateDemoVault();
        var service = new EntryMutationService();
        var entry = vault.Entries[0];
        var oldModified = entry.ModifiedUtc;
        var model = EntryEditModel.FromEntry(entry, "SASD-GmbH/IONOS");
        model.TagsText = "SASD, IONOS, Deployment";
        model.Title = "IONOS Webspace FTP Bearbeitet";

        service.UpdateEntry(vault, entry, model);

        Assert.Equal("IONOS Webspace FTP Bearbeitet", entry.Title);
        Assert.DoesNotContain("FTP", entry.Tags);
        Assert.Contains("Deployment", entry.Tags);
        Assert.True(entry.ModifiedUtc >= oldModified);
    }
}
