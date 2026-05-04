namespace Sasd.SecretManager.Storage;

/// <summary>
/// Fachliche Ausnahme für Probleme beim Speichern oder Laden eines Tresors.
/// </summary>
public sealed class VaultStorageException : Exception
{
    public VaultStorageException(string message)
        : base(message)
    {
    }

    public VaultStorageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
