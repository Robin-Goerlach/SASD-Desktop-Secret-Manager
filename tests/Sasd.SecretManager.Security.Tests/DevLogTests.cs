using Sasd.SecretManager.Security;
using Xunit;

namespace Sasd.SecretManager.Security.Tests;

public sealed class DevLogTests
{
    [Fact]
    public void WriteLine_Does_Not_Throw()
    {
        DevLog.WriteLine("Testmeldung");
        Assert.True(true);
    }
}
