using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Kleine Anwendungsdienst-Klasse, die einfache Zusammenfassungen für UI und Debug-Zwecke bereitstellt.
/// </summary>
public sealed class VaultSummaryService
{
    /// <summary>
    /// Erzeugt eine kurze lesbare Zusammenfassung eines Tresors.
    /// </summary>
    public string CreateSummary(SecretVault vault)
    {
        ArgumentNullException.ThrowIfNull(vault);

        // Diese erste Implementierung ist absichtlich schlicht.
        // Sie hilft uns aber dabei, Domain und UI früh lose zu koppeln.
        return $"{vault.Name}: {vault.Groups.Count} Gruppen, {vault.Entries.Count} Einträge, {vault.KnownTags.Count} bekannte Tags";
    }
}
