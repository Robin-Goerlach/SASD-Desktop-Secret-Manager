using Sasd.SecretManager.Domain;

// ============================================================================
// Dateiüberblick:
// Abstraktion für Laden und Speichern von Tresoren.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

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
