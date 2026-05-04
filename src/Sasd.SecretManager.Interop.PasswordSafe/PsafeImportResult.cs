using Sasd.SecretManager.Domain;

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
