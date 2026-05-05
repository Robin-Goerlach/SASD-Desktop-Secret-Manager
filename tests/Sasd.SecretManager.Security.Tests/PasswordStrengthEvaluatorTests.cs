using Sasd.SecretManager.Security;
using Xunit;

// ============================================================================
// Dateiüberblick:
// Enthält Unit-Tests zur Verifikation des beschriebenen Fach- und UI-nahen Verhaltens.
// Die Testnamen sind sprechend gewählt und dienen zugleich als Dokumentation
// des erwarteten Verhaltens der produktiven Klassen.
// ============================================================================

namespace Sasd.SecretManager.Security.Tests;

/// <summary>
    /// Testklasse für PasswordStrengthEvaluator und das dazugehörige erwartete Verhalten.
    /// </summary>
public sealed class PasswordStrengthEvaluatorTests
{
    /// <summary>
    /// Verifiziert: Assess ReturnsVeryWeakForShortSimplePassword.
    /// </summary>
    [Fact]
    public void Assess_ReturnsVeryWeakForShortSimplePassword()
    {
        var result = PasswordStrengthEvaluator.Assess("a");

        Assert.Equal(PasswordStrengthLevel.VeryWeak, result.Level);
        Assert.True(result.ShouldWarnBeforeUse);
    }

    /// <summary>
    /// Verifiziert: Assess ReturnsStrongForLongMixedPassword.
    /// </summary>
    [Fact]
    public void Assess_ReturnsStrongForLongMixedPassword()
    {
        var result = PasswordStrengthEvaluator.Assess("SASD-SecretManager-2026!Vault#Ready");

        Assert.Equal(PasswordStrengthLevel.Strong, result.Level);
        Assert.False(result.ShouldWarnBeforeUse);
    }
}
