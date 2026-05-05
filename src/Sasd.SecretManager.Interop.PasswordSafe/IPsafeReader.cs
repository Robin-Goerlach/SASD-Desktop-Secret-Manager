// ============================================================================
// Dateiüberblick:
// Definiert die spätere Reader-Schnittstelle für Password-Safe-Import.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.Interop.PasswordSafe;

/// <summary>
/// Platzhalter für einen zukünftigen Reader von Password-Safe-Dateien.
/// </summary>
public interface IPsafeReader
{
    /// <summary>
    /// Liest eine externe .psafe3-Datei in das interne Modell ein.
    /// </summary>
    Task<PsafeImportResult> ReadAsync(string filePath, string masterPassword, CancellationToken cancellationToken = default);
}
