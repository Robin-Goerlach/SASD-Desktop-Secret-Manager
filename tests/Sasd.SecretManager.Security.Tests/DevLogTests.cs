using Sasd.SecretManager.Security;
using Xunit;

// ============================================================================
// Dateiüberblick:
// Enthält Unit-Tests zur Verifikation des beschriebenen Fach- und UI-nahen Verhaltens.
// Die Testnamen sind sprechend gewählt und dienen zugleich als Dokumentation
// des erwarteten Verhaltens der produktiven Klassen.
// ============================================================================

namespace Sasd.SecretManager.Security.Tests;

/// <summary>
    /// Testklasse für DevLog und das dazugehörige erwartete Verhalten.
    /// </summary>
public sealed class DevLogTests
{
    /// <summary>
    /// Verifiziert: WriteLine Does Not Throw.
    /// </summary>
    [Fact]
    public void WriteLine_Does_Not_Throw()
    {
        DevLog.WriteLine("Testmeldung");
        Assert.True(true);
    }
}
