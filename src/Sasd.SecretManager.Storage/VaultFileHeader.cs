// ============================================================================
// Dateiüberblick:
// Beschreibt den technischen Header des Dateiformats.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.Storage;

/// <summary>
/// Minimaler Header des internen Tresorformats.
/// </summary>
public sealed class VaultFileHeader
{
    /// <summary>
    /// Kennung des Formats.
    /// </summary>
    public string Magic { get; set; } = VaultFileConstants.Magic;

    /// <summary>
    /// Formatversion der Datei.
    /// </summary>
    public int FormatVersion { get; set; } = VaultFileConstants.CurrentFormatVersion;

    /// <summary>
    /// Textuelle Beschreibung des KDF-Profils.
    /// </summary>
    public string KeyDerivationProfile { get; set; } = "PBKDF2-SHA256";

    /// <summary>
    /// Textuelle Beschreibung des Verschlüsselungsprofils.
    /// </summary>
    public string EncryptionProfile { get; set; } = "AES-256-GCM";
}
