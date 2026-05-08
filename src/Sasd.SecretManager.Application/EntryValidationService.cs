using System.Globalization;
using System.Net.Mail;
using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Fachlicher Validierungsservice für Eintragsdaten.
/// </summary>
/// <remarks>
/// DSM-003 führt diese Klasse ein, damit Eingaberegeln nicht mehr verstreut in
/// Dialogen, Importern oder Mutationsservices liegen. Der Service hat bewusst
/// keine Abhängigkeit auf WinForms und kann deshalb direkt in Unit-Tests, später
/// im Password-Safe-Import und in CLI-/Automatisierungswerkzeugen genutzt werden.
/// </remarks>
public sealed class EntryValidationService
{
    /// <summary>
    /// Maximal erlaubte Länge eines Eintragstitels.
    /// </summary>
    public const int MaxTitleLength = 160;

    /// <summary>
    /// Maximal erlaubte Länge eines Benutzernamens oder Login-Principals.
    /// </summary>
    public const int MaxUserNameLength = 320;

    /// <summary>
    /// Maximal erlaubte Länge eines einzelnen Tags.
    /// </summary>
    public const int MaxTagLength = 64;

    /// <summary>
    /// Maximal erlaubte Länge eines Zusatzfeldnamens.
    /// </summary>
    public const int MaxCustomFieldNameLength = 80;

    /// <summary>
    /// Maximal erlaubte Länge eines einzelnen Zusatzfeldwerts.
    /// </summary>
    public const int MaxCustomFieldValueLength = 4096;

    /// <summary>
    /// Prüft nur die in sich gültige Syntax eines Bearbeitungsmodells.
    /// </summary>
    /// <remarks>
    /// Diese Methode benötigt keinen Tresor. Sie eignet sich deshalb für den
    /// WinForms-Dialog, bevor der Dialog geschlossen wird. Tresorabhängige Regeln
    /// wie doppelte Titel oder unbekannte Gruppen werden in
    /// <see cref="ValidateForCreate"/> bzw. <see cref="ValidateForUpdate"/>
    /// zusätzlich geprüft.
    /// </remarks>
    public EntryValidationResult ValidateStandalone(EntryEditModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var result = new EntryValidationResult();
        ValidateCoreFields(model, result);
        ValidateTags(model.TagsText, result);
        ValidateCustomFields(model.CustomFieldsText, result);
        return result;
    }

    /// <summary>
    /// Prüft ein neues Eintragsmodell gegen den Ziel-Tresor.
    /// </summary>
    public EntryValidationResult ValidateForCreate(SecretVault vault, EntryEditModel model)
    {
        ArgumentNullException.ThrowIfNull(vault);
        ArgumentNullException.ThrowIfNull(model);

        var result = ValidateStandalone(model);
        var targetGroupId = ValidateAndResolveGroup(vault, model.SelectedGroupPath, result);
        ValidateDuplicateTitle(vault, model.Title, targetGroupId, excludingEntryId: null, result);
        return result;
    }

    /// <summary>
    /// Prüft ein geändertes Eintragsmodell gegen den Ziel-Tresor.
    /// </summary>
    public EntryValidationResult ValidateForUpdate(SecretVault vault, SecretEntry entry, EntryEditModel model)
    {
        ArgumentNullException.ThrowIfNull(vault);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(model);

        var result = ValidateStandalone(model);
        var targetGroupId = ValidateAndResolveGroup(vault, model.SelectedGroupPath, result);
        ValidateDuplicateTitle(vault, model.Title, targetGroupId, entry.Id, result);
        return result;
    }

    private static void ValidateCoreFields(EntryEditModel model, EntryValidationResult result)
    {
        var title = model.Title?.Trim() ?? string.Empty;
        if (title.Length == 0)
        {
            result.AddError(nameof(EntryEditModel.Title), "Bitte einen Titel angeben.");
        }
        else
        {
            if (title.Length > MaxTitleLength)
            {
                result.AddError(nameof(EntryEditModel.Title), $"Der Titel darf maximal {MaxTitleLength} Zeichen lang sein.");
            }

            if (ContainsLineBreak(title))
            {
                result.AddError(nameof(EntryEditModel.Title), "Der Titel darf keinen Zeilenumbruch enthalten.");
            }
        }

        var userName = model.UserName?.Trim() ?? string.Empty;
        if (userName.Length > MaxUserNameLength)
        {
            result.AddError(nameof(EntryEditModel.UserName), $"Der Benutzername darf maximal {MaxUserNameLength} Zeichen lang sein.");
        }

        if (ContainsLineBreak(userName))
        {
            result.AddError(nameof(EntryEditModel.UserName), "Der Benutzername darf keinen Zeilenumbruch enthalten.");
        }

        if (model.Secret is { Length: > 8192 })
        {
            result.AddError(nameof(EntryEditModel.Secret), "Das Secret ist ungewöhnlich lang. Bitte prüfen, ob der Wert wirklich in dieses Feld gehört.");
        }
    }

    private static void ValidateTags(string? tagsText, EntryValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(tagsText))
        {
            return;
        }

        var tags = tagsText
            .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .ToArray();

        var seenTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tag in tags)
        {
            if (tag.Length > MaxTagLength)
            {
                result.AddError(nameof(EntryEditModel.TagsText), $"Das Tag '{tag}' ist zu lang. Tags dürfen maximal {MaxTagLength} Zeichen haben.");
            }

            if (ContainsLineBreak(tag))
            {
                result.AddError(nameof(EntryEditModel.TagsText), $"Das Tag '{tag}' darf keinen Zeilenumbruch enthalten.");
            }

            if (!seenTags.Add(tag))
            {
                result.AddWarning(nameof(EntryEditModel.TagsText), $"Das Tag '{tag}' wurde mehrfach angegeben und wird nur einmal gespeichert.");
            }
        }
    }

    private static void ValidateCustomFields(string? customFieldsText, EntryValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(customFieldsText))
        {
            return;
        }

        var lines = customFieldsText
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Split('\n', StringSplitOptions.None);

        var seenFieldNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < lines.Length; index++)
        {
            var originalLine = lines[index];
            var line = originalLine.Trim();
            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            var splitResult = SplitCustomFieldLine(line);
            if (splitResult is null)
            {
                result.AddError(
                    nameof(EntryEditModel.CustomFieldsText),
                    $"Zusatzfeld Zeile {index + 1}: Bitte 'Name = Wert' oder 'Name: Wert' verwenden.");
                continue;
            }

            var (rawName, rawValue) = splitResult.Value;
            var isSecret = rawName.TrimStart().StartsWith('!');
            var normalizedName = rawName.Trim().TrimStart('!').Trim();
            var normalizedValue = rawValue.Trim();

            ValidateCustomFieldName(normalizedName, index + 1, seenFieldNames, result);
            ValidateCustomFieldValue(normalizedName, normalizedValue, isSecret, index + 1, result);
        }
    }

    private static void ValidateCustomFieldName(
        string normalizedName,
        int lineNumber,
        ISet<string> seenFieldNames,
        EntryValidationResult result)
    {
        if (normalizedName.Length == 0)
        {
            result.AddError(nameof(EntryEditModel.CustomFieldsText), $"Zusatzfeld Zeile {lineNumber}: Der Feldname darf nicht leer sein.");
            return;
        }

        if (normalizedName.Length > MaxCustomFieldNameLength)
        {
            result.AddError(
                nameof(EntryEditModel.CustomFieldsText),
                $"Zusatzfeld Zeile {lineNumber}: Der Feldname '{normalizedName}' ist zu lang. Maximal erlaubt sind {MaxCustomFieldNameLength} Zeichen.");
        }

        if (!seenFieldNames.Add(normalizedName))
        {
            result.AddError(
                nameof(EntryEditModel.CustomFieldsText),
                $"Zusatzfeld Zeile {lineNumber}: Das Feld '{normalizedName}' wurde mehrfach angegeben.");
        }
    }

    private static void ValidateCustomFieldValue(
        string name,
        string value,
        bool isSecret,
        int lineNumber,
        EntryValidationResult result)
    {
        if (value.Length > MaxCustomFieldValueLength)
        {
            result.AddError(
                nameof(EntryEditModel.CustomFieldsText),
                $"Zusatzfeld Zeile {lineNumber}: Der Wert für '{name}' ist zu lang.");
        }

        if (name.Length == 0)
        {
            return;
        }

        // Secret-Felder dürfen bewusst beliebige Inhalte enthalten. Ein API-Key
        // kann z. B. wie eine E-Mail-Adresse oder URL aussehen, ohne fachlich eine
        // Adresse zu sein. Deshalb prüfen wir Secret-Felder zurückhaltender.
        if (isSecret)
        {
            return;
        }

        if (LooksLikePortField(name))
        {
            ValidatePortValue(name, value, lineNumber, result);
        }

        if (LooksLikeUrlField(name, value))
        {
            ValidateUrlValue(name, value, lineNumber, result);
        }

        if (LooksLikeEmailField(name, value))
        {
            ValidateEmailValue(name, value, lineNumber, result);
        }
    }

    private static Guid? ValidateAndResolveGroup(SecretVault vault, string? groupPath, EntryValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(groupPath))
        {
            return null;
        }

        var group = vault.Groups.FirstOrDefault(item => string.Equals(item.Path, groupPath, StringComparison.OrdinalIgnoreCase));
        if (group is null)
        {
            result.AddError(nameof(EntryEditModel.SelectedGroupPath), "Die ausgewählte Gruppe existiert nicht mehr.");
            return null;
        }

        return group.Id;
    }

    private static void ValidateDuplicateTitle(
        SecretVault vault,
        string? title,
        Guid? targetGroupId,
        Guid? excludingEntryId,
        EntryValidationResult result)
    {
        var normalizedTitle = title?.Trim() ?? string.Empty;
        if (normalizedTitle.Length == 0)
        {
            return;
        }

        var duplicateExists = vault.Entries.Any(entry =>
            entry.Id != excludingEntryId
            && entry.GroupId == targetGroupId
            && string.Equals(entry.Title?.Trim(), normalizedTitle, StringComparison.OrdinalIgnoreCase));

        if (duplicateExists)
        {
            var groupText = targetGroupId is null ? "ohne Gruppe" : "in derselben Gruppe";
            result.AddError(
                nameof(EntryEditModel.Title),
                $"Es existiert bereits ein Eintrag mit dem Titel '{normalizedTitle}' {groupText}.");
        }
    }

    private static void ValidatePortValue(string name, string value, int lineNumber, EntryValidationResult result)
    {
        if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var port) || port is < 1 or > 65535)
        {
            result.AddError(
                nameof(EntryEditModel.CustomFieldsText),
                $"Zusatzfeld Zeile {lineNumber}: '{name}' muss eine Portnummer zwischen 1 und 65535 enthalten.");
        }
    }

    private static void ValidateUrlValue(string name, string value, int lineNumber, EntryValidationResult result)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || string.IsNullOrWhiteSpace(uri.Scheme))
        {
            result.AddError(
                nameof(EntryEditModel.CustomFieldsText),
                $"Zusatzfeld Zeile {lineNumber}: '{name}' sieht wie eine URL aus, ist aber keine gültige absolute URL.");
        }
    }

    private static void ValidateEmailValue(string name, string value, int lineNumber, EntryValidationResult result)
    {
        try
        {
            var mailAddress = new MailAddress(value);
            if (!string.Equals(mailAddress.Address, value, StringComparison.OrdinalIgnoreCase))
            {
                result.AddError(
                    nameof(EntryEditModel.CustomFieldsText),
                    $"Zusatzfeld Zeile {lineNumber}: '{name}' enthält keine einzelne gültige E-Mail-Adresse.");
            }
        }
        catch (FormatException)
        {
            result.AddError(
                nameof(EntryEditModel.CustomFieldsText),
                $"Zusatzfeld Zeile {lineNumber}: '{name}' enthält keine gültige E-Mail-Adresse.");
        }
    }

    private static (string Name, string Value)? SplitCustomFieldLine(string line)
    {
        var separatorIndex = line.IndexOf('=');
        if (separatorIndex < 0)
        {
            separatorIndex = line.IndexOf(':');
        }

        if (separatorIndex <= 0)
        {
            return null;
        }

        var name = line[..separatorIndex];
        var value = line[(separatorIndex + 1)..];
        return (name, value);
    }

    private static bool LooksLikePortField(string name)
    {
        return name.Contains("port", StringComparison.OrdinalIgnoreCase);
    }

    private static bool LooksLikeUrlField(string name, string value)
    {
        return name.Contains("url", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
    }

    private static bool LooksLikeEmailField(string name, string value)
    {
        return name.Contains("mail", StringComparison.OrdinalIgnoreCase)
            || name.Contains("e-mail", StringComparison.OrdinalIgnoreCase)
            || name.Contains("email", StringComparison.OrdinalIgnoreCase)
            || (value.Contains('@') && !value.Contains(' '));
    }

    private static bool ContainsLineBreak(string value)
    {
        return value.Contains('\r') || value.Contains('\n');
    }
}
