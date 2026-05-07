// ============================================================================
// Dateiüberblick:
// Technischer Persistenzcontainer für verschlüsselte Tresordaten.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.Storage;

/// <summary>
/// Serialisierbarer Container des internen .svault-Dateiformats.
/// Der eigentliche Tresorinhalt wird verschlüsselt als Base64 abgelegt.
/// </summary>
public sealed class VaultFileContainer
{
    public VaultFileHeader Header { get; set; } = new();
    public int Iterations { get; set; }
    public string SaltBase64 { get; set; } = string.Empty;
    public string NonceBase64 { get; set; } = string.Empty;
    public string TagBase64 { get; set; } = string.Empty;
    public string CipherTextBase64 { get; set; } = string.Empty;
    public DateTimeOffset SavedUtc { get; set; } = DateTimeOffset.UtcNow;
}
