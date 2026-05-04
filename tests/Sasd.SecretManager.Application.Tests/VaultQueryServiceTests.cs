using Sasd.SecretManager.Application;
using Xunit;

namespace Sasd.SecretManager.Application.Tests;

public sealed class VaultQueryServiceTests
{
    [Fact]
    public void GetVisibleEntries_FiltersByGroupAndSearch()
    {
        var vault = new DemoVaultFactory().CreateDemoVault();
        var service = new VaultQueryService();

        var results = service.GetVisibleEntries(vault, "SASD-GmbH/IONOS", "datenbank", 0, true);

        Assert.Single(results);
        Assert.Equal("SASD CMS Produktionsdatenbank", results[0].Title);
    }
}
