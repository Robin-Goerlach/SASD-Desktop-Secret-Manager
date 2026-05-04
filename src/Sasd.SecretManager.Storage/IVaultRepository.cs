using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Storage;

/// <summary>
/// Kapselt Dateioperationen für Tresore.
/// </summary>
public interface IVaultRepository
{
    /// <summary>
    /// Lädt einen Tresor aus einer Datei.
    /// </summary>
    Task<SecretVault> LoadAsync(string filePath, string masterPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Speichert einen Tresor in eine Datei.
    /// </summary>
    Task SaveAsync(SecretVault vault, string filePath, string masterPassword, CancellationToken cancellationToken = default);
}
