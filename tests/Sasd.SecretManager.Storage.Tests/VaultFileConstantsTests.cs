using Sasd.SecretManager.Storage;
using Xunit;

namespace Sasd.SecretManager.Storage.Tests;

public sealed class VaultFileConstantsTests
{
    [Fact]
    public void Magic_Is_SVLT()
    {
        Assert.Equal("SVLT", VaultFileConstants.Magic);
    }
}
