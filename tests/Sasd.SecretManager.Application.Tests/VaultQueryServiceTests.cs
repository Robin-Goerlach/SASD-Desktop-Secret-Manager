using Sasd.SecretManager.Application;
using Xunit;

// ============================================================================
// Dateiüberblick:
// Enthält Unit-Tests zur Verifikation des beschriebenen Fach- und UI-nahen Verhaltens.
// Die Testnamen sind sprechend gewählt und dienen zugleich als Dokumentation
// des erwarteten Verhaltens der produktiven Klassen.
// ============================================================================

namespace Sasd.SecretManager.Application.Tests;

/// <summary>
    /// Testklasse für VaultQueryService und das dazugehörige erwartete Verhalten.
    /// </summary>
public sealed class VaultQueryServiceTests
{
    /// <summary>
    /// Verifiziert: GetVisibleEntries FiltersByGroupAndSearch.
    /// </summary>
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
