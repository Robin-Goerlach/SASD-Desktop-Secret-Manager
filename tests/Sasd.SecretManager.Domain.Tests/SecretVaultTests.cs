using Sasd.SecretManager.Domain;
using Xunit;

namespace Sasd.SecretManager.Domain.Tests;

public sealed class SecretVaultTests
{
    [Fact]
    public void NewVault_Has_Default_ModelVersion_One()
    {
        var vault = new SecretVault();
        Assert.Equal(1, vault.ModelVersion);
    }
}
