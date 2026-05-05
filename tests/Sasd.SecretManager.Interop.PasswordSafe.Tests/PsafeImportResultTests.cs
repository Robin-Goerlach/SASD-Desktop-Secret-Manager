using Sasd.SecretManager.Interop.PasswordSafe;
using Xunit;

// ============================================================================
// Dateiüberblick:
// Enthält Unit-Tests zur Verifikation des beschriebenen Fach- und UI-nahen Verhaltens.
// Die Testnamen sind sprechend gewählt und dienen zugleich als Dokumentation
// des erwarteten Verhaltens der produktiven Klassen.
// ============================================================================

namespace Sasd.SecretManager.Interop.PasswordSafe.Tests;

/// <summary>
    /// Testklasse für PsafeImportResult und das dazugehörige erwartete Verhalten.
    /// </summary>
public sealed class PsafeImportResultTests
{
    /// <summary>
    /// Verifiziert: Warnings List Is Initialized.
    /// </summary>
    [Fact]
    public void Warnings_List_Is_Initialized()
    {
        var result = new PsafeImportResult();
        Assert.NotNull(result.Warnings);
        Assert.Empty(result.Warnings);
    }
}
