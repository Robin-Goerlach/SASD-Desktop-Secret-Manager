# Lastenheft: SASD Desktop Secret Manager

**Überarbeitete Fassung aus Dokumenten, Projektchats und Repository-Stand**  
Stand: 18.05.2026

> Dieses Lastenheft konsolidiert die bisherigen Projektunterlagen und Projektchats. Neu ausdrücklich aufgenommen ist die strukturierte Verwaltung von HTTPS/TLS-Zertifikaten.

## 1. Management Summary

Der SASD Desktop Secret Manager soll kein bloßer 1:1-Klon von Password Safe und keine einfache Passwortliste werden. Ziel ist ein lokaler, offline-first nutzbarer Windows-Desktop-Secret-Manager, der mehrere unabhängige verschlüsselte Tresore verwaltet und klassische Logins, technische Betriebszugänge, Tokens, Lizenzdaten, sichere Notizen und HTTPS/TLS-Zertifikatsinformationen strukturiert abbildet.

Das interne Datenmodell bleibt führend. Password-Safe-Interop wird additiv behandelt. Cloud-Sync, Team-Sharing, Mobile Clients und Browser-Autofill sind keine V1-Ziele.

## 2. Quellenbasis

Einbezogen wurden die hochgeladenen Lastenheft-, Pflichtenheft-, Architektur-, Featuremap- und Roadmap-Dokumente, der aktuelle GitHub-README-Stand sowie die relevanten Projektchats.

## 3. Kernanforderungen

- Lokale Windows-Desktop-Anwendung, offline-first, ohne Cloud-Zwang.
- Mehrere unabhängige Tresore mit eigenem Master-Passwort und eigener Datei.
- Strukturierte Einträge mit Typ, Titel, Benutzerkennung, Secret, Notizen, Gruppe, Tags, Zeitstempeln und Zusatzfeldern.
- Eintragstypen: Login, Database, Mail, Hosting, FTP/SFTP, Server, API, License, SecureNote, Custom und HTTPS/TLS-Zertifikat.
- Gruppen, Untergruppen, Tags, Root-/Tresorknoten, Suche, Filter, Kontextmenüs und Drag-and-Drop.
- Standardmäßig maskierte Secrets, bewusstes Reveal/Copy, Clipboard-Autoclear, Auto-Lock und keine Secrets in Logs.
- Robuste Dateiverwaltung, verschlüsselte Backups, keine unverschlüsselten temporären Klartextdateien.
- Password-Safe-Import aus .psafe3 mit Importbericht.
- Keine versteckte Recovery-Backdoor; spätere Recovery nur als offizieller, vorher eingerichteter Mechanismus.

## 4. HTTPS/TLS-Zertifikatsverwaltung

Zertifikate werden als eigener fachlicher Secret-Kontext aufgenommen. Ein Zertifikatseintrag soll strukturiert erfassen können:

- Domain, Common Name, Subject Alternative Names und Wildcard-Kennzeichnung.
- Aussteller, Seriennummer, Fingerprint/Thumbprint, gültig von und gültig bis.
- Format- und Ablagehinweise wie PEM, CRT, CER, PFX oder P12.
- Deployment-Ziel, Server-/Hosting-/Domain-Bezug und Erneuerungsverfahren.
- Private Schlüssel, PFX/P12-Passwort und Key-Passphrase als geheime Felder.
- Verantwortliche, Prüftermine und Hinweise zu Test-/Produktionsumgebungen.

Nicht V1-Ziel sind automatische ACME-Erneuerung, vollständige PKI-Verwaltung, Certificate-Transparency-Monitoring und automatische Server-Deployments.

## 5. Versionslogik

| Version | Zielcharakter | Inhalt |
|---|---|---|
| V0.x | Prototyp | Bestehender Entwicklungsstand mit Architektur, .svault, UI-Grundfunktionen, Gruppen, Tags, Suche, CRUD, Drag-and-Drop und Tests. |
| V1.0 | Runde lokale Erstversion | Mehrere Tresore, Secret-Typen, HTTPS/TLS-Zertifikatseintrag, sichere Anzeige, Clipboard-Autoclear, Auto-Lock, Passwortgenerator, Backups, .psafe3-Import. |
| V1.x | Härtung | Master-Passwort-Änderung, Restore-Sicherheit, Templates, Generatorprofile, Tag-Verwaltung, Produkticon, Zertifikatsablauf-Warnungen. |
| V2 | Ausbau | Mehrsprachigkeit, Themes, Cross-Vault, Historie, TOTP, verschlüsselte Anhänge, Zertifikatsdateien, optionale Online-Prüfungen. |
| V3/später | Heikle Erweiterungen | Browser-Import, Passkeys, ACME-/PKI-Automation, Team-/Cloud-/Mobile-Strategien. |

## 6. Bewusste Nicht-Ziele

- Kein vollständiger 1:1-Klon von Password Safe.
- Keine gruppenweiten Default-Passwörter.
- Keine versteckte Recovery-Hintertür.
- Keine Cloud-Synchronisation, Team-Sharing, Mobile Clients oder Browser-Autofill in V1.
- Keine automatische ACME-/PKI-Verwaltung in V1.

## 7. Abnahmekern V1

V1 ist fachlich abnahmefähig, wenn neue und bestehende Tresore sicher verwaltet werden, mehrere Tresore getrennt bleiben, die zentralen Secret-Typen inklusive HTTPS/TLS-Zertifikatseinträgen erfasst werden können, Suche und Organisation funktionieren, Secrets standardmäßig verborgen bleiben, Clipboard und Auto-Lock kontrolliert arbeiten, Backups verschlüsselt bleiben und der Password-Safe-Import nachvollziehbare Berichte erzeugt.
