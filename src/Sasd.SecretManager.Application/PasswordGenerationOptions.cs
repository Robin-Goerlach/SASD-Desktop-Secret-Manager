using System;
using System.Collections.Generic;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Beschreibt die fachlichen Einstellungen für den Passwortgenerator.
/// </summary>
/// <remarks>
/// Diese Klasse ist bewusst Teil der Application-Schicht und nicht der WinForms-UI.
/// Dadurch kann die Passwortgenerierung später auch aus anderen Oberflächen,
/// Tests oder Automatisierungen verwendet werden, ohne dass die fachliche Logik
/// an Windows Forms gekoppelt ist.
/// </remarks>
public sealed class PasswordGenerationOptions
{
    /// <summary>
    /// Kleinste sinnvolle Länge, die die Oberfläche im Normalfall anbieten soll.
    /// </summary>
    public const int MinimumLength = 8;

    /// <summary>
    /// Standardlänge für neue Passwörter.
    /// </summary>
    /// <remarks>
    /// 24 Zeichen sind für viele technische Zugänge ein guter Kompromiss:
    /// deutlich stärker als typische Benutzerpasswörter, aber noch handhabbar,
    /// falls ein Passwort ausnahmsweise manuell übertragen werden muss.
    /// </remarks>
    public const int DefaultLength = 24;

    /// <summary>
    /// Technische Obergrenze, damit versehentliche Extremwerte keine UI- oder
    /// Speicherprobleme verursachen.
    /// </summary>
    public const int MaximumLength = 256;

    /// <summary>
    /// Gewünschte Passwortlänge in Zeichen.
    /// </summary>
    public int Length { get; init; } = DefaultLength;

    /// <summary>
    /// Gibt an, ob Großbuchstaben verwendet werden sollen.
    /// </summary>
    public bool IncludeUppercase { get; init; } = true;

    /// <summary>
    /// Gibt an, ob Kleinbuchstaben verwendet werden sollen.
    /// </summary>
    public bool IncludeLowercase { get; init; } = true;

    /// <summary>
    /// Gibt an, ob Ziffern verwendet werden sollen.
    /// </summary>
    public bool IncludeDigits { get; init; } = true;

    /// <summary>
    /// Gibt an, ob Sonderzeichen verwendet werden sollen.
    /// </summary>
    public bool IncludeSymbols { get; init; } = true;

    /// <summary>
    /// Gibt an, ob leicht verwechselbare Zeichen ausgelassen werden sollen.
    /// </summary>
    /// <remarks>
    /// Beispiele sind 0/O, 1/l/I oder ähnliche Zeichen. Das reduziert die
    /// Verwechslungsgefahr, wenn Passwörter abgelesen oder telefonisch
    /// übertragen werden müssen. Für rein maschinelle Nutzung kann diese Option
    /// ausgeschaltet werden, um den Zeichenvorrat zu vergrößern.
    /// </remarks>
    public bool ExcludeAmbiguousCharacters { get; init; } = true;

    /// <summary>
    /// Erzwingt, dass aus jeder aktivierten Zeichengruppe mindestens ein Zeichen
    /// im Ergebnis enthalten ist.
    /// </summary>
    /// <remarks>
    /// Viele Zielsysteme verlangen mindestens einen Großbuchstaben, eine Ziffer
    /// oder ein Sonderzeichen. Diese Option macht das erzeugte Passwort für
    /// solche Systeme kompatibler. Der Generator prüft, ob die Länge dafür
    /// ausreicht.
    /// </remarks>
    public bool RequireEverySelectedCharacterGroup { get; init; } = true;

    /// <summary>
    /// Sonderzeichen, die bei aktivierter Symbol-Option verwendet werden dürfen.
    /// </summary>
    public string CustomSymbols { get; init; } = "!#$%&()*+,-./:;<=>?@[]^_{|}~";

    /// <summary>
    /// Prüft die Optionen und liefert sprechende Fehlermeldungen zurück.
    /// </summary>
    /// <returns>Eine Liste mit Validierungsfehlern. Eine leere Liste bedeutet: gültig.</returns>
    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();

        if (Length < MinimumLength)
        {
            errors.Add($"Die Passwortlänge muss mindestens {MinimumLength} Zeichen betragen.");
        }

        if (Length > MaximumLength)
        {
            errors.Add($"Die Passwortlänge darf höchstens {MaximumLength} Zeichen betragen.");
        }

        if (!IncludeUppercase && !IncludeLowercase && !IncludeDigits && !IncludeSymbols)
        {
            errors.Add("Es muss mindestens eine Zeichengruppe ausgewählt sein.");
        }

        if (IncludeSymbols && string.IsNullOrWhiteSpace(CustomSymbols))
        {
            errors.Add("Bei aktivierten Sonderzeichen muss mindestens ein Sonderzeichen erlaubt sein.");
        }

        return errors;
    }
}
