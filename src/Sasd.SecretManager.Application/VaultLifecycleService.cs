using Sasd.SecretManager.Domain;

// ============================================================================
// Dateiüberblick:
// Erzeugt neue Tresore und liefert einfache Default-Strukturen, damit die UI nicht direkt Domain-Objekte zusammensetzen muss.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.Application;

/// <summary>
/// Erzeugt neue leere Tresore mit sinnvoller Minimalstruktur.
/// </summary>
public sealed class VaultLifecycleService
{
    public SecretVault CreateNewVault(string vaultName)
    {
        var normalizedName = string.IsNullOrWhiteSpace(vaultName)
            ? "Neuer Tresor"
            : vaultName.Trim();

        var vault = new SecretVault
        {
            Name = normalizedName,
            ModelVersion = 1,
        };

        vault.Groups.Add(new EntryGroup
        {
            Name = "Allgemein",
            Path = "Allgemein",
        });

        return vault;
    }
}
