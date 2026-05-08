using System;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Ergebnis der Passwortgenerierung.
/// </summary>
/// <remarks>
/// Neben dem eigentlichen Passwort enthält das Ergebnis ein paar Metadaten,
/// die für Tests, Debugging und spätere UI-Hinweise nützlich sind. Das Passwort
/// wird aktuell noch als <see cref="string"/> zurückgegeben, weil die bestehende
/// Anwendung Secrets ebenfalls als Zeichenketten verarbeitet. Eine spätere
/// Härtung kann hier ansetzen und kurzlebigere Speicherformen einführen.
/// </remarks>
public sealed class GeneratedPassword
{
    /// <summary>
    /// Initialisiert ein neues Ergebnisobjekt.
    /// </summary>
    /// <param name="value">Das erzeugte Passwort.</param>
    /// <param name="options">Die Optionen, mit denen das Passwort erzeugt wurde.</param>
    /// <param name="selectedCharacterGroupCount">Anzahl der aktivierten Zeichengruppen.</param>
    public GeneratedPassword(string value, PasswordGenerationOptions options, int selectedCharacterGroupCount)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Options = options ?? throw new ArgumentNullException(nameof(options));
        SelectedCharacterGroupCount = selectedCharacterGroupCount;
        GeneratedAtUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Das erzeugte Passwort im Klartext.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Länge des erzeugten Passworts.
    /// </summary>
    public int Length => Value.Length;

    /// <summary>
    /// Optionen, mit denen das Passwort erzeugt wurde.
    /// </summary>
    public PasswordGenerationOptions Options { get; }

    /// <summary>
    /// Anzahl der aktivierten Zeichengruppen.
    /// </summary>
    public int SelectedCharacterGroupCount { get; }

    /// <summary>
    /// Zeitpunkt der Erzeugung in UTC.
    /// </summary>
    public DateTimeOffset GeneratedAtUtc { get; }
}
