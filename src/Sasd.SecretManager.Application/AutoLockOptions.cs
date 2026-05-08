namespace Sasd.SecretManager.Application;

/// <summary>
/// Fachliche Konfiguration für die automatische Sperrung eines geöffneten Tresors.
/// </summary>
/// <remarks>
/// Die Klasse liegt bewusst in der Application-Schicht, weil die Entscheidung
/// "wann soll gesperrt werden?" keine WinForms-Frage ist. Die konkrete Umsetzung
/// der Sperre, also Dialoge, Menüs und Timer, bleibt dagegen in der UI-Schicht.
/// </remarks>
public sealed record AutoLockOptions
{
    /// <summary>
    /// Standard-Timeout für die V1-Umsetzung.
    /// </summary>
    /// <remarks>
    /// Fünf Minuten sind für einen frühen Sicherheits-Milestone bewusst eher
    /// vorsichtig gewählt. Später kann daraus eine konfigurierbare Benutzereinstellung
    /// werden, z. B. 1, 5, 15 oder 30 Minuten.
    /// </remarks>
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Standard-Prüfintervall für den UI-Timer.
    /// </summary>
    public static readonly TimeSpan DefaultPollInterval = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gibt an, ob Auto-Lock aktiv ist.
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// Zeitspanne ohne Benutzeraktivität, nach der ein geöffneter Tresor gesperrt wird.
    /// </summary>
    public TimeSpan Timeout { get; init; } = DefaultTimeout;

    /// <summary>
    /// Intervall, in dem die UI prüfen soll, ob die Sperre fällig ist.
    /// </summary>
    public TimeSpan PollInterval { get; init; } = DefaultPollInterval;

    /// <summary>
    /// Liefert robuste Standardwerte, falls aus der UI oder später aus einer
    /// Konfigurationsdatei unbrauchbare Werte kommen.
    /// </summary>
    public AutoLockOptions Normalize()
    {
        var timeout = Timeout <= TimeSpan.Zero ? DefaultTimeout : Timeout;
        var pollInterval = PollInterval <= TimeSpan.Zero ? DefaultPollInterval : PollInterval;

        return this with
        {
            Timeout = timeout,
            PollInterval = pollInterval,
        };
    }
}
