using Sasd.SecretManager.Domain;
using Sasd.SecretManager.Storage;
using Xunit;

namespace Sasd.SecretManager.Storage.Tests;

public sealed class VaultFileRepositoryTests
{
    [Fact]
    public async Task SaveAndLoadAsync_RoundtripsVaultContent()
    {
        var repository = new VaultFileRepository();
        var vault = CreateSampleVault();
        var tempFile = Path.Combine(Path.GetTempPath(), $"sasd-vault-{Guid.NewGuid():N}.svault");

        try
        {
            await repository.SaveAsync(vault, tempFile, "Test!123456");
            var loadedVault = await repository.LoadAsync(tempFile, "Test!123456");

            Assert.Equal(vault.Name, loadedVault.Name);
            Assert.Equal(vault.Groups.Count, loadedVault.Groups.Count);
            Assert.Equal(vault.Entries.Count, loadedVault.Entries.Count);
            Assert.Equal(vault.Entries[0].Title, loadedVault.Entries[0].Title);
            Assert.Equal(vault.Entries[0].Secret, loadedVault.Entries[0].Secret);
        }
        finally
        {
            DeleteIfExists(tempFile);
            DeleteIfExists(tempFile + ".bak");
            DeleteIfExists(tempFile + ".tmp");
        }
    }

    [Fact]
    public async Task LoadAsync_WithWrongPassword_ThrowsVaultStorageException()
    {
        var repository = new VaultFileRepository();
        var vault = CreateSampleVault();
        var tempFile = Path.Combine(Path.GetTempPath(), $"sasd-vault-{Guid.NewGuid():N}.svault");

        try
        {
            await repository.SaveAsync(vault, tempFile, "Correct!123456");

            await Assert.ThrowsAsync<VaultStorageException>(
                () => repository.LoadAsync(tempFile, "Wrong!123456"));
        }
        finally
        {
            DeleteIfExists(tempFile);
            DeleteIfExists(tempFile + ".bak");
            DeleteIfExists(tempFile + ".tmp");
        }
    }

    private static SecretVault CreateSampleVault()
    {
        var rootGroup = new EntryGroup
        {
            Name = "Allgemein",
            Path = "Allgemein",
        };

        var entry = new SecretEntry
        {
            Title = "Testeintrag",
            EntryType = EntryType.Login,
            UserName = "tester",
            Secret = "Secret!123",
            GroupId = rootGroup.Id,
            Notes = "Nur für automatisierte Tests.",
            ModifiedUtc = DateTimeOffset.UtcNow,
        };
        entry.Tags.Add("Test");
        entry.CustomFields.Add(new CustomField
        {
            Name = "URL",
            Kind = CustomFieldKind.Url,
            Value = "https://example.invalid",
            SortOrder = 10,
        });

        return new SecretVault
        {
            Name = "Test Vault",
            Groups = [rootGroup],
            Entries = [entry],
            KnownTags = ["Test"],
        };
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
