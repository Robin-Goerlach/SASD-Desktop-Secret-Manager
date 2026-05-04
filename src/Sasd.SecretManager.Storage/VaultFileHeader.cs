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
    /// Freie textuelle Beschreibung des KDF-Profils.
    /// Die eigentlichen Sicherheitsparameter folgen erst in späteren Schritten.
    /// </summary>
    public string KeyDerivationProfile { get; set; } = "planned";
}
