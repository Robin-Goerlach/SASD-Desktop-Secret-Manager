using Sasd.SecretManager.Security;
using Xunit;

namespace Sasd.SecretManager.Security.Tests;

public sealed class PasswordStrengthEvaluatorTests
{
    [Fact]
    public void Assess_ReturnsVeryWeakForShortSimplePassword()
    {
        var result = PasswordStrengthEvaluator.Assess("a");

        Assert.Equal(PasswordStrengthLevel.VeryWeak, result.Level);
        Assert.True(result.ShouldWarnBeforeUse);
    }

    [Fact]
    public void Assess_ReturnsStrongForLongMixedPassword()
    {
        var result = PasswordStrengthEvaluator.Assess("SASD-SecretManager-2026!Vault#Ready");

        Assert.Equal(PasswordStrengthLevel.Strong, result.Level);
        Assert.False(result.ShouldWarnBeforeUse);
    }
}
