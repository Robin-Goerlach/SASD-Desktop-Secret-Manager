// ============================================================================
// Dateiüberblick:
// Spezielle Ausnahme für Datei-, Format- und Entschlüsselungsfehler.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

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

    /// <summary>
    /// Initialisiert die Ausnahme mit einer fachlich erklärenden Nachricht und der technischen Ursache.
    /// </summary>
    public VaultStorageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
