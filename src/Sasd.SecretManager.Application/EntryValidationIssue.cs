namespace Sasd.SecretManager.Application;

/// <summary>
/// Beschreibt ein einzelnes Validierungsergebnis für einen Eintrag.
/// </summary>
/// <remarks>
/// Die Anwendung verwendet bewusst ein kleines eigenes Modell anstelle von
/// UI-spezifischen Fehlermeldungen. Dadurch kann dieselbe Validierung sowohl
/// in WinForms als auch später in Importern, CLI-Werkzeugen oder Tests genutzt
/// werden. Der <see cref="PropertyName"/>-Wert ist fachlich gehalten und darf
/// von der UI zur Fokussteuerung oder Markierung verwendet werden.
/// </remarks>
public sealed class EntryValidationIssue
{
    /// <summary>
    /// Name des betroffenen Felds oder Eingabebereichs, z. B. "Title" oder
    /// "CustomFieldsText". Der Wert ist bewusst ein String, damit die Klasse
    /// keine Abhängigkeit auf WinForms-Steuerelemente bekommt.
    /// </summary>
    public string PropertyName { get; init; } = string.Empty;

    /// <summary>
    /// Menschlich lesbare deutschsprachige Meldung für Dialoge, Logs und Tests.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Kennzeichnet, ob der Befund das Speichern blockiert.
    /// </summary>
    public bool IsError { get; init; } = true;

    /// <summary>
    /// Kurze Textdarstellung für Debug-Ausgaben und Testfehler.
    /// </summary>
    public override string ToString()
    {
        var severity = IsError ? "Fehler" : "Hinweis";
        return string.IsNullOrWhiteSpace(PropertyName)
            ? $"{severity}: {Message}"
            : $"{severity} ({PropertyName}): {Message}";
    }
}
