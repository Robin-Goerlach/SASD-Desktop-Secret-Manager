using Sasd.SecretManager.Domain;

namespace Sasd.SecretManager.Application;

/// <summary>
/// Erzeugt einen bewusst kleinen In-Memory-Demo-Tresor.
/// Dieser Tresor dient in frühen UI-Ständen nur dazu,
/// TreeView, Listenansicht, Suche und Detaildarstellung realistisch zu testen.
/// </summary>
public sealed class DemoVaultFactory
{
    /// <summary>
    /// Erzeugt einen vollständigen Demo-Tresor mit Gruppen, Tags und Einträgen.
    /// </summary>
    public SecretVault CreateDemoVault()
    {
        var vault = new SecretVault
        {
            Name = "SASD Demo Vault",
        };

        // Gruppenhierarchie bewusst mit sprechenden Pfaden aufbauen.
        // Die Pfade helfen später bei Suche, Filterung und Interop.
        var sasd = new EntryGroup { Name = "SASD-GmbH", Path = "SASD-GmbH" };
        var ionos = new EntryGroup { Name = "IONOS", ParentGroupId = sasd.Id, Path = "SASD-GmbH/IONOS" };
        var mail = new EntryGroup { Name = "Mail", ParentGroupId = ionos.Id, Path = "SASD-GmbH/IONOS/Mail" };
        var databases = new EntryGroup { Name = "Datenbanken", ParentGroupId = ionos.Id, Path = "SASD-GmbH/IONOS/Datenbanken" };
        var github = new EntryGroup { Name = "GitHub", ParentGroupId = sasd.Id, Path = "SASD-GmbH/GitHub" };

        var privat = new EntryGroup { Name = "Privat", Path = "Privat" };
        var allgemein = new EntryGroup { Name = "Allgemein", ParentGroupId = privat.Id, Path = "Privat/Allgemein" };
        var finanzen = new EntryGroup { Name = "Finanzen", ParentGroupId = privat.Id, Path = "Privat/Finanzen" };

        vault.Groups.AddRange([sasd, ionos, mail, databases, github, privat, allgemein, finanzen]);
        vault.KnownTags.AddRange(["SASD", "IONOS", "GitHub", "Privat", "Produktion", "Mail", "Finanzen"]);

        vault.Entries.Add(CreateFtpEntry(ionos.Id));
        vault.Entries.Add(CreateDatabaseEntry(databases.Id));
        vault.Entries.Add(CreateGithubEntry(github.Id));
        vault.Entries.Add(CreateMailEntry(mail.Id));
        vault.Entries.Add(CreateFinanceEntry(finanzen.Id));
        vault.Entries.Add(CreateGeneralPrivateEntry(allgemein.Id));

        return vault;
    }

    private static SecretEntry CreateFtpEntry(Guid groupId)
    {
        var entry = new SecretEntry
        {
            Title = "IONOS Webspace FTP",
            EntryType = EntryType.Ftp,
            UserName = "deploy-user",
            Secret = "********",
            Notes = "Früher Demo-Eintrag für Deployment und Webspace-Uploads.",
            GroupId = groupId,
        };

        entry.Tags.AddRange(["SASD", "IONOS", "Deployment", "FTP"]);
        entry.CustomFields.AddRange(
        [
            new CustomField { Name = "Host", Kind = CustomFieldKind.Hostname, Value = "access-501234567.webspace-data.io", SortOrder = 10 },
            new CustomField { Name = "Port", Kind = CustomFieldKind.Port, Value = "21", SortOrder = 20 },
            new CustomField { Name = "Remote Path", Kind = CustomFieldKind.Text, Value = "/htdocs", SortOrder = 30 },
        ]);

        return entry;
    }

    private static SecretEntry CreateDatabaseEntry(Guid groupId)
    {
        var entry = new SecretEntry
        {
            Title = "SASD CMS Produktionsdatenbank",
            EntryType = EntryType.Database,
            UserName = "cms_prod",
            Secret = "********",
            Notes = "Produktionsdatenbank für das CMS. Später mit echter Tresorlogik verschlüsselt.",
            GroupId = groupId,
        };

        entry.Tags.AddRange(["SASD", "IONOS", "MySQL", "Produktion"]);
        entry.CustomFields.AddRange(
        [
            new CustomField { Name = "Host", Kind = CustomFieldKind.Hostname, Value = "db123456.hosting-data.io", SortOrder = 10 },
            new CustomField { Name = "Port", Kind = CustomFieldKind.Port, Value = "3306", SortOrder = 20 },
            new CustomField { Name = "Datenbank", Kind = CustomFieldKind.Text, Value = "sasd_cms_prod", SortOrder = 30 },
            new CustomField { Name = "SSL", Kind = CustomFieldKind.Boolean, Value = "true", SortOrder = 40 },
        ]);

        return entry;
    }

    private static SecretEntry CreateGithubEntry(Guid groupId)
    {
        var entry = new SecretEntry
        {
            Title = "GitHub Organisation",
            EntryType = EntryType.Login,
            UserName = "Robin-Goerlach",
            Secret = "********",
            Notes = "Persönliches Entwicklungs-Repository für frühe Stände; Release-Stände später separat.",
            GroupId = groupId,
        };

        entry.Tags.AddRange(["GitHub", "SASD", "Code", "Organisation"]);
        entry.CustomFields.AddRange(
        [
            new CustomField { Name = "URL", Kind = CustomFieldKind.Url, Value = "https://github.com/Robin-Goerlach/SASD-Desktop-Secret-Manager", SortOrder = 10 },
            new CustomField { Name = "Rolle", Kind = CustomFieldKind.Text, Value = "Owner", SortOrder = 20 },
        ]);

        return entry;
    }

    private static SecretEntry CreateMailEntry(Guid groupId)
    {
        var entry = new SecretEntry
        {
            Title = "support@sasd-gmbh.de",
            EntryType = EntryType.Mail,
            UserName = "support@sasd-gmbh.de",
            Secret = "********",
            Notes = "Support-Postfach bei IONOS mit klassischer IMAP/SMTP-Konfiguration.",
            GroupId = groupId,
        };

        entry.Tags.AddRange(["SASD", "IONOS", "Mail", "Support"]);
        entry.CustomFields.AddRange(
        [
            new CustomField { Name = "IMAP Host", Kind = CustomFieldKind.Hostname, Value = "imap.ionos.de", SortOrder = 10 },
            new CustomField { Name = "IMAP Port", Kind = CustomFieldKind.Port, Value = "993", SortOrder = 20 },
            new CustomField { Name = "SMTP Host", Kind = CustomFieldKind.Hostname, Value = "smtp.ionos.de", SortOrder = 30 },
            new CustomField { Name = "SMTP Port", Kind = CustomFieldKind.Port, Value = "587", SortOrder = 40 },
        ]);

        return entry;
    }

    private static SecretEntry CreateFinanceEntry(Guid groupId)
    {
        var entry = new SecretEntry
        {
            Title = "Privates Banking Portal",
            EntryType = EntryType.Login,
            UserName = "robin.private",
            Secret = "********",
            Notes = "Privater Eintrag für Finanzen. Dient hier nur als Demo für getrennte Bereiche.",
            GroupId = groupId,
        };

        entry.Tags.AddRange(["Privat", "Finanzen", "Banking"]);
        entry.CustomFields.AddRange(
        [
            new CustomField { Name = "URL", Kind = CustomFieldKind.Url, Value = "https://bank.example.invalid", SortOrder = 10 },
            new CustomField { Name = "2FA", Kind = CustomFieldKind.Boolean, Value = "true", SortOrder = 20 },
        ]);

        return entry;
    }

    private static SecretEntry CreateGeneralPrivateEntry(Guid groupId)
    {
        var entry = new SecretEntry
        {
            Title = "Privater Mailzugang",
            EntryType = EntryType.Mail,
            UserName = "robin@example.invalid",
            Secret = "********",
            Notes = "Kleiner Demonstrationseintrag für den privaten Bereich.",
            GroupId = groupId,
        };

        entry.Tags.AddRange(["Privat", "Mail"]);
        entry.CustomFields.AddRange(
        [
            new CustomField { Name = "Provider", Kind = CustomFieldKind.Text, Value = "Example Mail", SortOrder = 10 },
            new CustomField { Name = "URL", Kind = CustomFieldKind.Url, Value = "https://mail.example.invalid", SortOrder = 20 },
        ]);

        return entry;
    }
}
