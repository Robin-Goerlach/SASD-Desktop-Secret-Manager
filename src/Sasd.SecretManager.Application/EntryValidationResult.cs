namespace Sasd.SecretManager.Application;

/// <summary>
/// Sammelobjekt für das Ergebnis einer Eintragsvalidierung.
/// </summary>
/// <remarks>
/// Ein Ergebnis kann harte Fehler und weichere Hinweise enthalten. Für DSM-003
/// werden nur harte Fehler zum Blockieren des Speicherns verwendet. Die Trennung
/// ist aber absichtlich schon vorbereitet, weil spätere Meilensteine Warnungen
/// anzeigen können, ohne den Benutzer unnötig zu blockieren.
/// </remarks>
public sealed class EntryValidationResult
{
    private readonly List<EntryValidationIssue> _issues = new();

    /// <summary>
    /// Alle gefundenen Validierungsbefunde in der Reihenfolge ihrer Erkennung.
    /// </summary>
    public IReadOnlyList<EntryValidationIssue> Issues => _issues;

    /// <summary>
    /// Nur die Befunde, die das Speichern verhindern.
    /// </summary>
    public IReadOnlyList<EntryValidationIssue> Errors => _issues.Where(issue => issue.IsError).ToArray();

    /// <summary>
    /// Nur die Hinweise, die später optional angezeigt werden können.
    /// </summary>
    public IReadOnlyList<EntryValidationIssue> Warnings => _issues.Where(issue => !issue.IsError).ToArray();

    /// <summary>
    /// True, wenn keine harten Fehler gefunden wurden.
    /// </summary>
    public bool IsValid => !_issues.Any(issue => issue.IsError);

    /// <summary>
    /// Fügt einen harten Fehler hinzu.
    /// </summary>
    public void AddError(string propertyName, string message)
    {
        _issues.Add(new EntryValidationIssue
        {
            PropertyName = propertyName,
            Message = message,
            IsError = true,
        });
    }

    /// <summary>
    /// Fügt einen nicht blockierenden Hinweis hinzu.
    /// </summary>
    public void AddWarning(string propertyName, string message)
    {
        _issues.Add(new EntryValidationIssue
        {
            PropertyName = propertyName,
            Message = message,
            IsError = false,
        });
    }

    /// <summary>
    /// Wirft eine <see cref="EntryValidationException"/>, falls harte Fehler
    /// vorhanden sind. Application-Services können dadurch schlank bleiben.
    /// </summary>
    public void ThrowIfInvalid()
    {
        if (!IsValid)
        {
            throw new EntryValidationException(Errors);
        }
    }
}
