namespace Sasd.SecretManager.Storage;

/// <summary>
/// Enthält feste Kennungen für das interne Tresorformat.
/// </summary>
public static class VaultFileConstants
{
    /// <summary>
    /// Magic Header des internen Formats.
    /// </summary>
    public const string Magic = "SVLT";

    /// <summary>
    /// Erste geplante Formatversion.
    /// </summary>
    public const int CurrentFormatVersion = 1;
}
