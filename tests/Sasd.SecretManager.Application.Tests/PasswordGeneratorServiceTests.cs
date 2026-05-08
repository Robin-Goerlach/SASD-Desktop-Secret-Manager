using Sasd.SecretManager.Application;
using Xunit;

namespace Sasd.SecretManager.Application.Tests;

/// <summary>
/// Tests für den fachlichen Passwortgenerator.
/// </summary>
/// <remarks>
/// Die Tests prüfen bewusst nicht auf ein konkretes Passwort. Das wäre bei
/// kryptographischem Zufall falsch. Geprüft werden die fachlichen Zusagen:
/// Länge, Zeichengruppen, Validierung und Ausschluss verwechselbarer Zeichen.
/// </remarks>
public sealed class PasswordGeneratorServiceTests
{
    private readonly PasswordGeneratorService _service = new();

    [Fact]
    public void Generate_ReturnsPasswordWithRequestedLength()
    {
        var result = _service.Generate(new PasswordGenerationOptions
        {
            Length = 32,
        });

        Assert.Equal(32, result.Length);
        Assert.Equal(32, result.Value.Length);
    }

    [Fact]
    public void Generate_IncludesEverySelectedCharacterGroup_WhenRequired()
    {
        var result = _service.Generate(new PasswordGenerationOptions
        {
            Length = 32,
            IncludeUppercase = true,
            IncludeLowercase = true,
            IncludeDigits = true,
            IncludeSymbols = true,
            ExcludeAmbiguousCharacters = false,
            RequireEverySelectedCharacterGroup = true,
            CustomSymbols = "!#%",
        });

        Assert.Contains(result.Value, char.IsUpper);
        Assert.Contains(result.Value, char.IsLower);
        Assert.Contains(result.Value, char.IsDigit);
        Assert.Contains(result.Value, character => "!#%".Contains(character));
    }

    [Fact]
    public void Generate_Throws_WhenNoCharacterGroupIsSelected()
    {
        var exception = Assert.Throws<ArgumentException>(() => _service.Generate(new PasswordGenerationOptions
        {
            IncludeUppercase = false,
            IncludeLowercase = false,
            IncludeDigits = false,
            IncludeSymbols = false,
        }));

        Assert.Contains("Zeichengruppe", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Generate_Throws_WhenLengthIsTooShort()
    {
        var exception = Assert.Throws<ArgumentException>(() => _service.Generate(new PasswordGenerationOptions
        {
            Length = 4,
        }));

        Assert.Contains("mindestens", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Generate_ExcludesAmbiguousCharacters_WhenOptionIsEnabled()
    {
        var result = _service.Generate(new PasswordGenerationOptions
        {
            Length = 80,
            IncludeUppercase = true,
            IncludeLowercase = true,
            IncludeDigits = true,
            IncludeSymbols = false,
            ExcludeAmbiguousCharacters = true,
            RequireEverySelectedCharacterGroup = true,
        });

        Assert.DoesNotContain('0', result.Value);
        Assert.DoesNotContain('O', result.Value);
        Assert.DoesNotContain('1', result.Value);
        Assert.DoesNotContain('l', result.Value);
        Assert.DoesNotContain('I', result.Value);
    }
}
