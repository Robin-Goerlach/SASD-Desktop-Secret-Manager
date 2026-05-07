// ============================================================================
// Dateiüberblick:
// Kapselt die testbare Fachlogik rund um ungespeicherte Änderungen.
// Die Klasse entscheidet bewusst nur über Bedarf, Text und Ergebnis einer
// Rückfrage. Die eigentliche UI-Anzeige der MessageBox bleibt in WinForms.
// ============================================================================

namespace Sasd.SecretManager.Application;

/// <summary>
/// Stellt testbare Regeln für Rückfragen bei ungespeicherten Änderungen bereit.
/// </summary>
/// <remarks>
/// Die Klasse hält die eigentliche Entscheidungslogik aus der WinForms-Oberfläche heraus,
/// damit die Regeln für <c>Neu</c>, <c>Öffnen</c>, <c>Beenden</c> und das Schließen des
/// Fensters ohne UI-Abhängigkeit geprüft werden können.
/// </remarks>
public sealed class UnsavedChangesGuardService
{
    /// <summary>
    /// Ermittelt, ob vor einer Navigation oder Abschlussaktion eine Rückfrage nötig ist.
    /// </summary>
    /// <param name="isDirty">Gibt an, ob der aktuelle Tresor ungespeicherte Änderungen enthält.</param>
    /// <returns><see langword="true"/>, wenn eine Rückfrage nötig ist; andernfalls <see langword="false"/>.</returns>
    public bool RequiresConfirmation(bool isDirty) => isDirty;

    /// <summary>
    /// Baut den Rückfragetext passend zum Auslöser der Navigation.
    /// </summary>
    /// <param name="vaultName">Optionaler Anzeigename des aktuellen Tresors.</param>
    /// <param name="action">Die Aktion, für die eine Rückfrage angezeigt wird.</param>
    /// <returns>Ein verständlicher Rückfragetext für die Oberfläche.</returns>
    public string BuildConfirmationMessage(string? vaultName, UnsavedChangesNavigationAction action)
    {
        var displayName = string.IsNullOrWhiteSpace(vaultName) ? "dieser Tresor" : $"der Tresor '{vaultName.Trim()}'";
        var actionLabel = action switch
        {
            UnsavedChangesNavigationAction.CreateNewVault => "vor dem Anlegen eines neuen Tresors",
            UnsavedChangesNavigationAction.OpenVault => "vor dem Öffnen eines anderen Tresors",
            UnsavedChangesNavigationAction.ExitApplication => "vor dem Beenden der Anwendung",
            UnsavedChangesNavigationAction.CloseWindow => "vor dem Schließen des Fensters",
            _ => "vor dem Fortfahren",
        };

        return $"{displayName} enthält ungespeicherte Änderungen. Möchtest du ihn {actionLabel} speichern?";
    }

    /// <summary>
    /// Wandelt die Benutzerauswahl in eine fachliche Entscheidung um.
    /// </summary>
    /// <param name="isDirty">Gibt an, ob aktuell ungespeicherte Änderungen vorliegen.</param>
    /// <param name="choice">Die aufbereitete Benutzerauswahl aus der UI.</param>
    /// <returns>Die fachliche Folgeentscheidung für die aufrufende Oberfläche.</returns>
    public UnsavedChangesGuardDecision Evaluate(bool isDirty, UnsavedChangesPromptChoice choice)
    {
        if (!isDirty)
        {
            return UnsavedChangesGuardDecision.ContinueWithoutSaving;
        }

        return choice switch
        {
            UnsavedChangesPromptChoice.Save => UnsavedChangesGuardDecision.SaveBeforeContinuing,
            UnsavedChangesPromptChoice.Discard => UnsavedChangesGuardDecision.ContinueWithoutSaving,
            _ => UnsavedChangesGuardDecision.Cancel,
        };
    }
}

/// <summary>
/// Beschreibt, welcher UI-Ablauf gerade ungespeicherte Änderungen gefährden könnte.
/// </summary>
public enum UnsavedChangesNavigationAction
{
    /// <summary>
    /// Ein neuer Tresor soll angelegt werden.
    /// </summary>
    CreateNewVault,

    /// <summary>
    /// Ein vorhandener Tresor soll geöffnet werden.
    /// </summary>
    OpenVault,

    /// <summary>
    /// Die Anwendung soll über einen expliziten Beenden-Befehl verlassen werden.
    /// </summary>
    ExitApplication,

    /// <summary>
    /// Das Fenster wird geschlossen, etwa über das Fenstersystem oder Alt+F4.
    /// </summary>
    CloseWindow,
}

/// <summary>
/// Beschreibt die vom Benutzer getroffene Antwort auf die Rückfrage.
/// </summary>
public enum UnsavedChangesPromptChoice
{
    /// <summary>
    /// Änderungen speichern und dann fortfahren.
    /// </summary>
    Save,

    /// <summary>
    /// Änderungen verwerfen und direkt fortfahren.
    /// </summary>
    Discard,

    /// <summary>
    /// Den Vorgang abbrechen.
    /// </summary>
    Cancel,
}

/// <summary>
/// Beschreibt die fachliche Folgeentscheidung nach der Rückfrage.
/// </summary>
public enum UnsavedChangesGuardDecision
{
    /// <summary>
    /// Ohne zusätzliches Speichern fortfahren.
    /// </summary>
    ContinueWithoutSaving,

    /// <summary>
    /// Vor dem Fortfahren zuerst speichern.
    /// </summary>
    SaveBeforeContinuing,

    /// <summary>
    /// Den laufenden Vorgang abbrechen.
    /// </summary>
    Cancel,
}
