using Sasd.SecretManager.Application;
using Xunit;

namespace Sasd.SecretManager.Application.Tests;

/// <summary>
/// Tests für die fachliche Auto-Lock-Entscheidung.
/// </summary>
public sealed class AutoLockServiceTests
{
    private readonly AutoLockService _service = new();

    [Fact]
    public void ShouldLock_ReturnsTrue_WhenTimeoutIsReached()
    {
        var now = DateTimeOffset.UtcNow;
        var options = new AutoLockOptions
        {
            Timeout = TimeSpan.FromMinutes(5),
        };

        var shouldLock = _service.ShouldLock(
            options,
            lastUserActivityUtc: now.AddMinutes(-5),
            nowUtc: now,
            hasOpenVault: true,
            isAlreadyLocked: false);

        Assert.True(shouldLock);
    }

    [Fact]
    public void ShouldLock_ReturnsFalse_WhenVaultIsAlreadyLocked()
    {
        var now = DateTimeOffset.UtcNow;

        var shouldLock = _service.ShouldLock(
            new AutoLockOptions(),
            lastUserActivityUtc: now.AddHours(-1),
            nowUtc: now,
            hasOpenVault: true,
            isAlreadyLocked: true);

        Assert.False(shouldLock);
    }

    [Fact]
    public void ShouldLock_ReturnsFalse_WhenAutoLockIsDisabled()
    {
        var now = DateTimeOffset.UtcNow;
        var options = new AutoLockOptions
        {
            IsEnabled = false,
            Timeout = TimeSpan.FromMinutes(1),
        };

        var shouldLock = _service.ShouldLock(
            options,
            lastUserActivityUtc: now.AddMinutes(-10),
            nowUtc: now,
            hasOpenVault: true,
            isAlreadyLocked: false);

        Assert.False(shouldLock);
    }

    [Fact]
    public void GetRemainingTime_ReturnsZero_WhenTimeoutHasPassed()
    {
        var now = DateTimeOffset.UtcNow;
        var options = new AutoLockOptions
        {
            Timeout = TimeSpan.FromMinutes(5),
        };

        var remaining = _service.GetRemainingTime(
            options,
            lastUserActivityUtc: now.AddMinutes(-6),
            nowUtc: now);

        Assert.Equal(TimeSpan.Zero, remaining);
    }
}
