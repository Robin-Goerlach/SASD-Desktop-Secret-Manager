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
    /// Testklasse für VaultLifecycleService und das dazugehörige erwartete Verhalten.
    /// </summary>
public sealed class VaultLifecycleServiceTests
{
    /// <summary>
    /// Verifiziert: CreateNewVault CreatesDefaultGeneralGroup.
    /// </summary>
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
