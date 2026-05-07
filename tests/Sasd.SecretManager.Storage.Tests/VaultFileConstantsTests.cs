using Sasd.SecretManager.Storage;
using Xunit;

// ============================================================================
// Dateiüberblick:
// Enthält Unit-Tests zur Verifikation des beschriebenen Fach- und UI-nahen Verhaltens.
// Die Testnamen sind sprechend gewählt und dienen zugleich als Dokumentation
// des erwarteten Verhaltens der produktiven Klassen.
// ============================================================================

namespace Sasd.SecretManager.Storage.Tests;

/// <summary>
    /// Testklasse für VaultFileConstants und das dazugehörige erwartete Verhalten.
    /// </summary>
public sealed class VaultFileConstantsTests
{
    /// <summary>
    /// Verifiziert: Magic Is SVLT.
    /// </summary>
    [Fact]
    public void Magic_Is_SVLT()
    {
        Assert.Equal("SVLT", VaultFileConstants.Magic);
    }
}
