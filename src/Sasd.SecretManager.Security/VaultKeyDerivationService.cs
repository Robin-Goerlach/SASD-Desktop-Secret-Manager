using System.Security.Cryptography;
using System.Text;

// ============================================================================
// Dateiüberblick:
// Leitet aus Master-Passwort und Salt den Verschlüsselungsschlüssel ab.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.Security;

/// <summary>
/// Leitet aus einem Master-Passwort einen symmetrischen Schlüssel ab.
/// Für Milestone 4 nutzen wir bewusst PBKDF2-SHA256 als stabile .NET-Basis.
/// </summary>
public sealed class VaultKeyDerivationService
{
    public const int DefaultIterations = 600_000;
    public const int SaltSize = 16;
    public const int KeySize = 32;

    /// <summary>
    /// Erzeugt ein frisches Salt für die Schlüsselableitung.
    /// </summary>
    public byte[] CreateRandomSalt()
    {
        return RandomNumberGenerator.GetBytes(SaltSize);
    }

    /// <summary>
    /// Leitet aus Master-Passwort und Salt einen stabilen Verschlüsselungsschlüssel ab.
    /// </summary>
    public byte[] DeriveKey(string masterPassword, byte[] salt, int iterations = DefaultIterations)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(masterPassword);
        ArgumentNullException.ThrowIfNull(salt);

        return Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(masterPassword),
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            KeySize);
    }
}
