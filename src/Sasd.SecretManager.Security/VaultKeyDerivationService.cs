using System.Security.Cryptography;
using System.Text;

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

    public byte[] CreateRandomSalt()
    {
        return RandomNumberGenerator.GetBytes(SaltSize);
    }

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
