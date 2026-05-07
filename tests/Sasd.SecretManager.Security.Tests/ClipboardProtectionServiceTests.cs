using Sasd.SecretManager.Security;
using Xunit;

// ============================================================================
// Dateiüberblick:
// Verifiziert das fachliche Verhalten der Clipboard-Schutzlogik. Die Tests
// prüfen bewusst nur Policies und keine WinForms-spezifische Timer- oder
// Clipboard-Integration.
// ============================================================================

namespace Sasd.SecretManager.Security.Tests;

/// <summary>
/// Testklasse für ClipboardProtectionService und die zugehörigen Schutzrichtlinien.
/// </summary>
public sealed class ClipboardProtectionServiceTests
{
    /// <summary>
    /// Verifiziert: GetPolicy ReturnsAutoClearForSecret.
    /// </summary>
    [Fact]
    public void GetPolicy_ReturnsAutoClearForSecret()
    {
        var service = new ClipboardProtectionService();

        var policy = service.GetPolicy(ClipboardCopyKind.Secret);

        Assert.True(policy.IsSensitive);
        Assert.True(policy.ShouldAutoClear);
        Assert.True(policy.AutoClearDelay > TimeSpan.Zero);
        Assert.Contains("Secret", policy.CopiedMessage);
    }

    /// <summary>
    /// Verifiziert: GetPolicy ReturnsAutoClearForSecretCustomField.
    /// </summary>
    [Fact]
    public void GetPolicy_ReturnsAutoClearForSecretCustomField()
    {
        var service = new ClipboardProtectionService();

        var policy = service.GetPolicy(ClipboardCopyKind.SecretCustomField);

        Assert.True(policy.IsSensitive);
        Assert.True(policy.ShouldAutoClear);
        Assert.True(policy.AutoClearDelay > TimeSpan.Zero);
        Assert.Contains("Zwischenablage", policy.ClearedMessage);
    }

    /// <summary>
    /// Verifiziert: GetPolicy ReturnsNoAutoClearForUrl.
    /// </summary>
    [Fact]
    public void GetPolicy_ReturnsNoAutoClearForUrl()
    {
        var service = new ClipboardProtectionService();

        var policy = service.GetPolicy(ClipboardCopyKind.Url);

        Assert.False(policy.IsSensitive);
        Assert.False(policy.ShouldAutoClear);
        Assert.Equal(TimeSpan.Zero, policy.AutoClearDelay);
    }

    /// <summary>
    /// Verifiziert: GetPolicy ReturnsNoAutoClearForUserName.
    /// </summary>
    [Fact]
    public void GetPolicy_ReturnsNoAutoClearForUserName()
    {
        var service = new ClipboardProtectionService();

        var policy = service.GetPolicy(ClipboardCopyKind.UserName);

        Assert.False(policy.IsSensitive);
        Assert.False(policy.ShouldAutoClear);
        Assert.Contains("Benutzername", policy.CopiedMessage);
    }
}
