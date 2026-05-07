// ============================================================================
// Dateiüberblick:
// Ergebnisobjekt für die Bewertung der Passwortstärke.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.Security;

/// <summary>
/// Ergebnis einer einfachen Passwortbewertung für UI-Warnungen.
/// </summary>
public sealed class PasswordStrengthAssessment
{
    /// <summary>
    /// Qualitätseinstufung des geprüften Passworts.
    /// </summary>
    public PasswordStrengthLevel Level { get; init; }

    /// <summary>
    /// Knappes Benutzerfeedback zur Bewertung.
    /// </summary>
    public string Summary { get; init; } = string.Empty;

    /// <summary>
    /// Zusätzlicher kurzer Verbesserungshinweis.
    /// </summary>
    public string Recommendation { get; init; } = string.Empty;

    /// <summary>
    /// Rohscore für Vergleiche und Tests.
    /// </summary>
    public int Score { get; init; }

    /// <summary>
    /// Kennzeichnet, ob die UI vor der Verwendung aktiv warnen sollte.
    /// </summary>
    public bool ShouldWarnBeforeUse => Level is PasswordStrengthLevel.VeryWeak or PasswordStrengthLevel.Weak;
}
