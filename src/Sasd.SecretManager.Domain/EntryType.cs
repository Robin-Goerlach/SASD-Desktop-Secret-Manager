// ============================================================================
// Dateiüberblick:
// Beschreibt die fachliche Art eines Eintrags, etwa Login, Mailkonto oder API-Zugang.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.Domain;

/// <summary>
/// Beschreibt den fachlichen Typ eines sicheren Eintrags.
/// </summary>
public enum EntryType
{
    Login = 0,
    Database = 1,
    Server = 2,
    Mail = 3,
    Ftp = 4,
    ApiKey = 5,
    License = 6,
    SecureNote = 7,
    Custom = 99,
}
