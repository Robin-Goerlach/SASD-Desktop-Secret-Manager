using Sasd.SecretManager.Application;
using Xunit;

// ============================================================================
// Dateiüberblick:
// Verifiziert die UI-arme Entscheidungslogik für ungespeicherte Änderungen.
// Die Tests sichern bewusst nur die fachliche Policy ab; die konkrete Anzeige
// der MessageBox verbleibt in der WinForms-Schicht.
// ============================================================================

namespace Sasd.SecretManager.Application.Tests;

/// <summary>
/// Testet die Regeln des <see cref="UnsavedChangesGuardService"/>.
/// </summary>
public sealed class UnsavedChangesGuardServiceTests
{
    private readonly UnsavedChangesGuardService _service = new();

    /// <summary>
    /// Verifiziert, dass bei sauberem Tresor keine Rückfrage nötig ist.
    /// </summary>
    [Fact]
    public void RequiresConfirmation_ReturnsFalse_WhenVaultIsClean()
    {
        Assert.False(_service.RequiresConfirmation(false));
    }

    /// <summary>
    /// Verifiziert, dass bei ungespeicherten Änderungen eine Rückfrage ausgelöst wird.
    /// </summary>
    [Fact]
    public void RequiresConfirmation_ReturnsTrue_WhenVaultIsDirty()
    {
        Assert.True(_service.RequiresConfirmation(true));
    }

    /// <summary>
    /// Verifiziert, dass die Rückfrage beim Öffnen eines anderen Tresors den Tresornamen enthält.
    /// </summary>
    [Fact]
    public void BuildConfirmationMessage_UsesVaultName_AndActionSpecificText()
    {
        var message = _service.BuildConfirmationMessage("SASD Demo Vault", UnsavedChangesNavigationAction.OpenVault);

        Assert.Contains("SASD Demo Vault", message);
        Assert.Contains("vor dem Öffnen eines anderen Tresors", message);
    }

    /// <summary>
    /// Verifiziert, dass ohne Tresornamen ein neutraler Ersatztext genutzt wird.
    /// </summary>
    [Fact]
    public void BuildConfirmationMessage_UsesFallbackName_WhenVaultNameIsMissing()
    {
        var message = _service.BuildConfirmationMessage(null, UnsavedChangesNavigationAction.CloseWindow);

        Assert.Contains("dieser Tresor", message);
        Assert.Contains("vor dem Schließen des Fensters", message);
    }

    /// <summary>
    /// Verifiziert, dass die Auswahl Speichern in die Entscheidung "zuerst speichern" übersetzt wird.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsSaveBeforeContinuing_WhenUserChoosesSave()
    {
        var result = _service.Evaluate(true, UnsavedChangesPromptChoice.Save);

        Assert.Equal(UnsavedChangesGuardDecision.SaveBeforeContinuing, result);
    }

    /// <summary>
    /// Verifiziert, dass Verwerfen direkt zum Fortfahren ohne Speichern führt.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsContinueWithoutSaving_WhenUserChoosesDiscard()
    {
        var result = _service.Evaluate(true, UnsavedChangesPromptChoice.Discard);

        Assert.Equal(UnsavedChangesGuardDecision.ContinueWithoutSaving, result);
    }

    /// <summary>
    /// Verifiziert, dass Abbrechen den laufenden Vorgang blockiert.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsCancel_WhenUserChoosesCancel()
    {
        var result = _service.Evaluate(true, UnsavedChangesPromptChoice.Cancel);

        Assert.Equal(UnsavedChangesGuardDecision.Cancel, result);
    }

    /// <summary>
    /// Verifiziert, dass ein sauberer Tresor unabhängig von der UI-Auswahl ohne Nachfrage fortgeführt werden darf.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsContinueWithoutSaving_WhenVaultIsNotDirty()
    {
        var result = _service.Evaluate(false, UnsavedChangesPromptChoice.Cancel);

        Assert.Equal(UnsavedChangesGuardDecision.ContinueWithoutSaving, result);
    }
}
