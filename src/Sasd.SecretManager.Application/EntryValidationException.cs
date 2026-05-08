namespace Sasd.SecretManager.Application;

/// <summary>
/// Ausnahme für fachlich ungültige Eintragsdaten.
/// </summary>
/// <remarks>
/// Diese Ausnahme ist kein technischer Fehler, sondern ein kontrollierter
/// Validierungsabbruch. Die UI soll daraus eine verständliche Warnmeldung bauen,
/// statt die Ausnahme ungefiltert als Crash oder generische Fehlermeldung zu
/// zeigen.
/// </remarks>
public sealed class EntryValidationException : InvalidOperationException
{
    /// <summary>
    /// Alle harten Validierungsfehler, die zum Abbruch geführt haben.
    /// </summary>
    public IReadOnlyList<EntryValidationIssue> Issues { get; }

    public EntryValidationException(IReadOnlyList<EntryValidationIssue> issues)
        : base(BuildMessage(issues))
    {
        Issues = issues.ToArray();
    }

    private static string BuildMessage(IReadOnlyList<EntryValidationIssue> issues)
    {
        if (issues.Count == 0)
        {
            return "Der Eintrag ist ungültig.";
        }

        return "Der Eintrag ist ungültig: " + string.Join("; ", issues.Select(issue => issue.Message));
    }
}
