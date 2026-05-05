// ============================================================================
// Dateiüberblick:
// Bewertet Passwörter heuristisch, um den Benutzer frühzeitig auf schwache Kennwörter hinzuweisen.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.Security;

/// <summary>
/// Bewertet Passwörter mit einer bewusst einfachen, transparenten Heuristik.
/// Die Bewertung ersetzt keine echte Sicherheitsgarantie,
/// hilft aber, triviale Master-Passwörter früh zu erkennen.
/// </summary>
public static class PasswordStrengthEvaluator
{
    public static PasswordStrengthAssessment Assess(string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return new PasswordStrengthAssessment
            {
                Level = PasswordStrengthLevel.VeryWeak,
                Summary = "Sehr schwach",
                Recommendation = "Bitte ein nicht-leeres Master-Passwort vergeben.",
                Score = 0,
            };
        }

        var length = password.Length;
        var hasLower = password.Any(char.IsLower);
        var hasUpper = password.Any(char.IsUpper);
        var hasDigit = password.Any(char.IsDigit);
        var hasSymbol = password.Any(character => !char.IsLetterOrDigit(character) && !char.IsWhiteSpace(character));
        var containsWhitespace = password.Any(char.IsWhiteSpace);
        var distinctCharacterCount = password.Distinct().Count();
        var categoryCount = (hasLower ? 1 : 0) + (hasUpper ? 1 : 0) + (hasDigit ? 1 : 0) + (hasSymbol ? 1 : 0);

        var score = 0;

        if (length >= 8)
        {
            score += 1;
        }

        if (length >= 12)
        {
            score += 1;
        }

        if (length >= 16)
        {
            score += 1;
        }

        if (categoryCount >= 2)
        {
            score += 1;
        }

        if (categoryCount >= 3)
        {
            score += 1;
        }

        if (categoryCount >= 4)
        {
            score += 1;
        }

        if (distinctCharacterCount >= 8)
        {
            score += 1;
        }

        if (distinctCharacterCount >= 12)
        {
            score += 1;
        }

        if (containsWhitespace)
        {
            score -= 1;
        }

        score = Math.Clamp(score, 0, 8);

        var level = score switch
        {
            <= 1 => PasswordStrengthLevel.VeryWeak,
            <= 3 => PasswordStrengthLevel.Weak,
            <= 5 => PasswordStrengthLevel.Moderate,
            <= 7 => PasswordStrengthLevel.Good,
            _ => PasswordStrengthLevel.Strong,
        };

        var summary = level switch
        {
            PasswordStrengthLevel.VeryWeak => "Sehr schwach",
            PasswordStrengthLevel.Weak => "Schwach",
            PasswordStrengthLevel.Moderate => "Mittel",
            PasswordStrengthLevel.Good => "Gut",
            PasswordStrengthLevel.Strong => "Stark",
            _ => "Unbekannt",
        };

        var recommendation = BuildRecommendation(length, categoryCount, containsWhitespace, distinctCharacterCount);

        return new PasswordStrengthAssessment
        {
            Level = level,
            Summary = summary,
            Recommendation = recommendation,
            Score = score,
        };
    }

    private static string BuildRecommendation(int length, int categoryCount, bool containsWhitespace, int distinctCharacterCount)
    {
        if (length < 12)
        {
            return "Mindestens 12 Zeichen verwenden; besser 16 oder mehr.";
        }

        if (categoryCount < 3)
        {
            return "Groß-/Kleinbuchstaben, Ziffern und Sonderzeichen besser mischen.";
        }

        if (distinctCharacterCount < 10)
        {
            return "Mehr unterschiedliche Zeichen verwenden und Wiederholungen reduzieren.";
        }

        if (containsWhitespace)
        {
            return "Leerzeichen vermeiden, wenn der Tresor auch in einfachen Umgebungen genutzt werden soll.";
        }

        return "Für V1 wirkt dieses Master-Passwort solide.";
    }
}
