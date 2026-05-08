using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Erzeugt kryptographisch zufällige Passwörter anhand fachlicher Optionen.
/// </summary>
/// <remarks>
/// Der Service verwendet bewusst <see cref="RandomNumberGenerator"/> und nicht
/// <see cref="Random"/>. <see cref="Random"/> ist für Spiele, Simulationen oder
/// nicht-sicherheitskritische Zufallswerte geeignet, aber nicht für Secrets.
///
/// Die Klasse ist zustandslos. Jede Generierung kann unabhängig getestet und
/// verwendet werden. Das passt zur bestehenden SASD-Schichtung: Die UI sammelt
/// Optionen ein, die Application-Schicht erzeugt das Passwort.
/// </remarks>
public sealed class PasswordGeneratorService
{
    private const string UppercaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string LowercaseCharacters = "abcdefghijklmnopqrstuvwxyz";
    private const string DigitCharacters = "0123456789";

    /// <summary>
    /// Zeichen, die beim manuellen Lesen leicht verwechselt werden können.
    /// </summary>
    private static readonly HashSet<char> AmbiguousCharacters = new("0O1lI|`'\"");

    /// <summary>
    /// Erzeugt ein neues Passwort.
    /// </summary>
    /// <param name="options">Optionen für Länge und Zeichengruppen.</param>
    /// <returns>Das erzeugte Passwort samt Metadaten.</returns>
    /// <exception cref="ArgumentNullException">Wird ausgelöst, wenn <paramref name="options"/> null ist.</exception>
    /// <exception cref="ArgumentException">Wird ausgelöst, wenn die Optionen fachlich ungültig sind.</exception>
    public GeneratedPassword Generate(PasswordGenerationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var validationErrors = options.Validate();
        if (validationErrors.Count > 0)
        {
            throw new ArgumentException(string.Join(Environment.NewLine, validationErrors), nameof(options));
        }

        var characterGroups = BuildCharacterGroups(options);
        if (characterGroups.Count == 0)
        {
            // Diese Absicherung sollte durch Validate() bereits abgefangen sein.
            // Sie bleibt trotzdem hier, weil die eigentliche Generierung niemals
            // mit leerem Zeichenvorrat arbeiten darf.
            throw new ArgumentException("Es wurde kein verwendbarer Zeichenvorrat ermittelt.", nameof(options));
        }

        if (options.RequireEverySelectedCharacterGroup && options.Length < characterGroups.Count)
        {
            throw new ArgumentException(
                "Die Passwortlänge ist zu kurz, um aus jeder gewählten Zeichengruppe mindestens ein Zeichen zu verwenden.",
                nameof(options));
        }

        var allCharacters = string.Concat(characterGroups).ToCharArray();
        var passwordCharacters = new List<char>(capacity: options.Length);

        if (options.RequireEverySelectedCharacterGroup)
        {
            // Zuerst je ein Zeichen aus jeder aktivierten Gruppe ziehen.
            // Danach wird aufgefüllt und anschließend gemischt. Dadurch steht
            // fest, dass die gewünschten Gruppen vertreten sind, ohne dass die
            // Reihenfolge des Passworts vorhersehbar wird.
            foreach (var group in characterGroups)
            {
                passwordCharacters.Add(GetRandomCharacter(group));
            }
        }

        while (passwordCharacters.Count < options.Length)
        {
            passwordCharacters.Add(GetRandomCharacter(allCharacters));
        }

        ShuffleInPlace(passwordCharacters);

        return new GeneratedPassword(
            value: new string(passwordCharacters.ToArray()),
            options: options,
            selectedCharacterGroupCount: characterGroups.Count);
    }

    /// <summary>
    /// Baut die aktivierten Zeichengruppen auf und entfernt bei Bedarf
    /// verwechselbare Zeichen.
    /// </summary>
    private static List<char[]> BuildCharacterGroups(PasswordGenerationOptions options)
    {
        var groups = new List<char[]>();

        AddGroupIfEnabled(groups, UppercaseCharacters, options.IncludeUppercase, options.ExcludeAmbiguousCharacters);
        AddGroupIfEnabled(groups, LowercaseCharacters, options.IncludeLowercase, options.ExcludeAmbiguousCharacters);
        AddGroupIfEnabled(groups, DigitCharacters, options.IncludeDigits, options.ExcludeAmbiguousCharacters);
        AddGroupIfEnabled(groups, options.CustomSymbols, options.IncludeSymbols, options.ExcludeAmbiguousCharacters);

        return groups;
    }

    /// <summary>
    /// Fügt eine Zeichengruppe hinzu, wenn sie aktiviert ist und nach der
    /// Bereinigung noch mindestens ein Zeichen enthält.
    /// </summary>
    private static void AddGroupIfEnabled(
        ICollection<char[]> groups,
        string characters,
        bool isEnabled,
        bool excludeAmbiguousCharacters)
    {
        if (!isEnabled)
        {
            return;
        }

        var sanitizedCharacters = characters
            .Where(character => !char.IsWhiteSpace(character))
            .Where(character => !excludeAmbiguousCharacters || !AmbiguousCharacters.Contains(character))
            .Distinct()
            .ToArray();

        if (sanitizedCharacters.Length == 0)
        {
            throw new ArgumentException("Eine aktivierte Zeichengruppe enthält nach der Bereinigung keine verwendbaren Zeichen mehr.");
        }

        groups.Add(sanitizedCharacters);
    }

    /// <summary>
    /// Wählt ein Zeichen mit kryptographisch sicherem Zufall aus.
    /// </summary>
    private static char GetRandomCharacter(IReadOnlyList<char> characters)
    {
        var index = RandomNumberGenerator.GetInt32(characters.Count);
        return characters[index];
    }

    /// <summary>
    /// Mischt die Zeichen mit Fisher-Yates und kryptographisch sicherem Zufall.
    /// </summary>
    private static void ShuffleInPlace(IList<char> characters)
    {
        for (var index = characters.Count - 1; index > 0; index--)
        {
            var swapIndex = RandomNumberGenerator.GetInt32(index + 1);
            (characters[index], characters[swapIndex]) = (characters[swapIndex], characters[index]);
        }
    }
}
