namespace Sasd.SecretManager.Application;

/// <summary>
/// Kleine, testbare Fachlogik für Auto-Lock.
/// </summary>
/// <remarks>
/// Diese Klasse kennt keine Windows-Forms-Controls und keine Passwörter. Sie
/// entscheidet lediglich, ob anhand von letzter Benutzeraktivität, aktuellem
/// Zeitpunkt und Tresorzustand gesperrt werden soll. Dadurch ist die Logik leicht
/// unit-testbar und kann später auch in einer anderen Oberfläche wiederverwendet
/// werden.
/// </remarks>
public sealed class AutoLockService
{
    /// <summary>
    /// Prüft, ob ein geöffneter Tresor jetzt automatisch gesperrt werden soll.
    /// </summary>
    /// <param name="options">Auto-Lock-Konfiguration.</param>
    /// <param name="lastUserActivityUtc">Zeitpunkt der letzten registrierten Benutzeraktivität in UTC.</param>
    /// <param name="nowUtc">Aktueller Zeitpunkt in UTC.</param>
    /// <param name="hasOpenVault">Gibt an, ob überhaupt ein Tresor geöffnet ist.</param>
    /// <param name="isAlreadyLocked">Gibt an, ob der Tresor bereits gesperrt ist.</param>
    /// <returns><see langword="true"/>, wenn Auto-Lock jetzt ausgelöst werden soll.</returns>
    public bool ShouldLock(
        AutoLockOptions? options,
        DateTimeOffset lastUserActivityUtc,
        DateTimeOffset nowUtc,
        bool hasOpenVault,
        bool isAlreadyLocked)
    {
        var normalizedOptions = (options ?? new AutoLockOptions()).Normalize();

        if (!normalizedOptions.IsEnabled || !hasOpenVault || isAlreadyLocked)
        {
            return false;
        }

        if (nowUtc < lastUserActivityUtc)
        {
            return false;
        }

        return nowUtc - lastUserActivityUtc >= normalizedOptions.Timeout;
    }

    /// <summary>
    /// Berechnet, wie lange es bei unveränderter Inaktivität noch bis zur Sperre dauert.
    /// </summary>
    public TimeSpan GetRemainingTime(
        AutoLockOptions? options,
        DateTimeOffset lastUserActivityUtc,
        DateTimeOffset nowUtc)
    {
        var normalizedOptions = (options ?? new AutoLockOptions()).Normalize();

        if (!normalizedOptions.IsEnabled || nowUtc < lastUserActivityUtc)
        {
            return normalizedOptions.Timeout;
        }

        var remaining = normalizedOptions.Timeout - (nowUtc - lastUserActivityUtc);
        return remaining <= TimeSpan.Zero ? TimeSpan.Zero : remaining;
    }
}
