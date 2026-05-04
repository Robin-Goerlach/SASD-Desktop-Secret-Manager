using Sasd.SecretManager.Application;
using Xunit;

namespace Sasd.SecretManager.Application.Tests;

public sealed class VaultLifecycleServiceTests
{
    [Fact]
    public void CreateNewVault_CreatesDefaultGeneralGroup()
    {
        var service = new VaultLifecycleService();

        var vault = service.CreateNewVault("Mein Tresor");

        Assert.Equal("Mein Tresor", vault.Name);
        Assert.Single(vault.Groups);
        Assert.Equal("Allgemein", vault.Groups[0].Name);
        Assert.Equal("Allgemein", vault.Groups[0].Path);
    }
}
