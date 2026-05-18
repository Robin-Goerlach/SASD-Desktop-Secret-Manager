# Repository-Metadaten und Außendarstellung

## Kurzbeschreibung für GitHub

Lokaler Windows Desktop Secret Manager in C#/.NET 8 mit verschlüsselten `.svault`-Tresoren, strukturierten technischen Secrets, Password-Safe-Interop und geplanter HTTPS/TLS-Zertifikatsverwaltung.

## Längere Beschreibung

SASD Desktop Secret Manager ist ein lokal orientierter Secret Manager für Windows. Das Projekt verwaltet klassische Logins, technische Zugangsdaten, API-Secrets, Datenbank- und Hosting-Zugänge sowie strukturierte Zusatzinformationen in verschlüsselten Tresoren. Das interne Modell bleibt führend; Password Safe wird als Import-/Interop-Schicht betrachtet. Der aktuelle Stand ist ein aktiver Entwicklungsstand und noch kein produktiv freigegebenes Sicherheitsprodukt.

## Empfohlene Topics

```text
csharp
dotnet
winforms
password-manager
secret-manager
credential-management
local-first
offline-first
security
encryption
vault
password-safe
keepass-alternative
desktop-app
sasd
```

Nach Aufnahme der Zertifikatsfunktionen zusätzlich:

```text
certificate-management
tls-certificates
pki
```

## Website / Projektlink

Solange keine stabile Produktseite existiert, sollte kein Marketing-Link gesetzt werden. Später kann eine SASD-GmbH-Projektseite ergänzt werden.

## README-Schwerpunkte

Die README-Datei sollte folgende Punkte sichtbar machen:

- lokaler Desktop Secret Manager, nicht Cloud-Dienst,
- aktueller Entwicklungsstatus,
- Build/Test-Anleitung,
- Sicherheitswarnung,
- Dokumentationslinks,
- V1-Roadmap,
- klare Nicht-Ziele,
- Lizenz.

## Warnhinweis für frühe Nutzer

> Dieses Projekt ist sicherheitskritisch und noch in Entwicklung. Es sollte nicht als alleiniger produktiver Speicher für echte Geheimnisse verwendet werden, solange V1-Security-Gates, Testkatalog und Release-Härtung nicht abgeschlossen sind.
