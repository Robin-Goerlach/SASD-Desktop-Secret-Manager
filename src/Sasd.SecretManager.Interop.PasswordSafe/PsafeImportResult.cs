using Sasd.SecretManager.Domain;

// ============================================================================
// Dateiüberblick:
// Hält das Ergebnis eines Password-Safe-Imports inklusive Warnungen.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.Interop.PasswordSafe;

/// <summary>
/// Ergebnisobjekt für einen späteren Import aus Password Safe.
/// </summary>
public sealed class PsafeImportResult
{
    /// <summary>
    /// Importierter Tresor in internem Modell.
    /// </summary>
    public SecretVault? Vault { get; set; }

    /// <summary>
    /// Hinweise zu Feldern oder Informationen, die nicht 1:1 überführt werden konnten.
    /// </summary>
    public List<string> Warnings { get; } = new();
}
