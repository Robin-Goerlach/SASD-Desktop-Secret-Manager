using System.Security.Cryptography;

// ============================================================================
// Dateiüberblick:
// Kapselt die symmetrische Ver- und Entschlüsselung des Tresorformats.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.Security;

/// <summary>
/// Kapselt symmetrische Verschlüsselung und Entschlüsselung für Tresordaten.
/// Für Milestone 4 verwenden wir AES-256-GCM mit zufälliger Nonce pro Speichervorgang.
/// </summary>
public sealed class VaultEncryptionService
{
    public const int NonceSize = 12;
    public const int TagSize = 16;

    /// <summary>
    /// Erzeugt einen kryptografisch zufälligen Nonce für AES-GCM.
    /// </summary>
    public byte[] CreateRandomNonce()
    {
        return RandomNumberGenerator.GetBytes(NonceSize);
    }

    /// <summary>
    /// Verschlüsselt Nutzdaten und liefert zusätzlich den Authentifizierungstag zurück.
    /// </summary>
    public byte[] Encrypt(byte[] plainBytes, byte[] key, byte[] nonce, out byte[] tag)
    {
        ArgumentNullException.ThrowIfNull(plainBytes);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(nonce);

        var cipherBytes = new byte[plainBytes.Length];
        tag = new byte[TagSize];

        using var aes = new AesGcm(key, TagSize);
        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);
        return cipherBytes;
    }

    /// <summary>
    /// Entschlüsselt einen mit AES-GCM verschlüsselten Datenblock.
    /// </summary>
    public byte[] Decrypt(byte[] cipherBytes, byte[] key, byte[] nonce, byte[] tag)
    {
        ArgumentNullException.ThrowIfNull(cipherBytes);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(nonce);
        ArgumentNullException.ThrowIfNull(tag);

        var plainBytes = new byte[cipherBytes.Length];
        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, cipherBytes, tag, plainBytes);
        return plainBytes;
    }
}
