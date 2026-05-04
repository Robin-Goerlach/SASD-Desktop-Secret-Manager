using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Storage;

/// <summary>
/// Kapselt spätere Dateioperationen für Tresore.
/// </summary>
public interface IVaultRepository
{
    /// <summary>
    /// Lädt einen Tresor aus einer Datei.
    /// </summary>
    Task<SecretVault> LoadAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Speichert einen Tresor in eine Datei.
    /// </summary>
    Task SaveAsync(SecretVault vault, string filePath, CancellationToken cancellationToken = default);
}
