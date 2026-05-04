using Sasd.SecretManager.Interop.PasswordSafe;
using Xunit;

namespace Sasd.SecretManager.Interop.PasswordSafe.Tests;

public sealed class PsafeImportResultTests
{
    [Fact]
    public void Warnings_List_Is_Initialized()
    {
        var result = new PsafeImportResult();
        Assert.NotNull(result.Warnings);
        Assert.Empty(result.Warnings);
    }
}
