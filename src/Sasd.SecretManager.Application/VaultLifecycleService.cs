using Sasd.SecretManager.Domain;

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
