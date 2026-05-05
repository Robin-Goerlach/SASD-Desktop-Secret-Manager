using Sasd.SecretManager.Domain;
using Xunit;

// ============================================================================
// Dateiüberblick:
// Enthält Unit-Tests zur Verifikation des beschriebenen Fach- und UI-nahen Verhaltens.
// Die Testnamen sind sprechend gewählt und dienen zugleich als Dokumentation
// des erwarteten Verhaltens der produktiven Klassen.
// ============================================================================

namespace Sasd.SecretManager.Domain.Tests;

/// <summary>
    /// Testklasse für SecretVault und das dazugehörige erwartete Verhalten.
    /// </summary>
public sealed class SecretVaultTests
{
    /// <summary>
    /// Verifiziert: NewVault Has Default ModelVersion One.
    /// </summary>
    [Fact]
    public void NewVault_Has_Default_ModelVersion_One()
    {
        var vault = new SecretVault();
        Assert.Equal(1, vault.ModelVersion);
    }
}
