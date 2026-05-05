# SASD Desktop Secret Manager

Technisch sauberer, lokal orientierter Desktop-Secret-Manager für Windows auf Basis von C#/.NET und WinForms. Das Projekt bündelt die bisher erarbeiteten fachlichen und architektonischen Grundlagen und dient zunächst als **persönliches Entwicklungs-Repository**. Spätere, aufgeräumte Release-Stände können separat in ein SASD-GmbH-Repository überführt werden.

> **Status:** Konzept- und Vorbereitungsphase. Noch **nicht** für produktive Geheimnisverwaltung einsetzen.

## Visuelle Richtung

![Konzept-Screenshot der Anwendung](assets/readme/app-concept.png)

Die Anwendung ist als ruhige, dunkle Desktop-Oberfläche mit Drei-Spalten-Layout gedacht:
- links Tresore, Gruppen und Tags,
- mittig Such- und Ergebnisbereich,
- rechts Detailansicht des ausgewählten Eintrags.

## Projektidee

Die Anwendung soll **mehr sein als ein einfacher Passwortmanager**. Ziel ist ein lokaler Secret Manager mit verschlüsselten Tresoren, der neben klassischen Logins auch technische Zugänge strukturiert verwalten kann, etwa:

- Hosting-Backends
- Datenbankverbindungen
- FTP/SFTP-Zugänge
- Mailkonten
- API-Tokens
- GitHub-Zugänge
- Server- und Infrastruktur-Zugänge
- weitere technische Zusatzinformationen über flexible Felder

## Leitlinien

- **eigenes internes Datenmodell** als führende Fachstruktur
- **mehrere unabhängige Tresore**
- **Gruppen + Tags** zur Organisation
- **starke Verschlüsselung** und klare Sicherheitsgrenzen
- **Password Safe Interop** als zusätzliche Kompatibilitätsschicht, nicht als Primärformat
- saubere Trennung von **Domain**, **Application**, **Security**, **Storage**, **Interop** und **UI**
- stark kommentierter, nachvollziehbarer Code

## Geplanter Release-Schnitt

### V1
- internes Tresorformat (`.svault`)
- lokaler Einzelbenutzerbetrieb
- Master-Passwort
- mehrere unabhängige Tresore
- Gruppen, Tags, Suche
- Eintragstypen und Zusatzfelder
- Passwortgenerator
- Auto-Lock und Clipboard Auto-Clear
- atomisches Speichern und verschlüsselte Backups
- `.psafe3`-Import

### V1.x
- Härtung, Testausbau und Komfort
- `.psafe3`-Export mit Kompatibilitätsbericht
- besseres Mehrtresor-Handling
- Vorlagen, Generatorprofile, zusätzliche Assistenten

### V2.0
- weiter ausgebauter Kompatibilitätsmodus
- Passwort-Historie
- spätere Recovery-Strategien ohne Backdoor
- mögliche Mehrtresor-Suche und fortgeschrittene Verwaltungsfunktionen

## Repository-Strategie

Aktuell ist eine **Entwicklung unter dem persönlichen GitHub-Account** vorgesehen. Dort können Zwischenschritte, Iterationen und experimentellere Entwicklungsstände bleiben. Bereinigte Release-Stände können später in ein separates Repository unter `sasdgmbh` übernommen werden.

Mehr dazu in:
- [`docs/repository-strategy.md`](docs/repository-strategy.md)
- [`docs/repository-metadata.md`](docs/repository-metadata.md)

## Dokumente

- [`docs/lastenheft/`](docs/lastenheft/)
- [`docs/pflichtenheft/`](docs/pflichtenheft/)
- [`docs/architektur/`](docs/architektur/)
- [`docs/roadmap.md`](docs/roadmap.md)
- [`docs/decisions/`](docs/decisions/)


## Archiv früherer Projektstände

- [`archive/prototype/`](archive/prototype/) enthält einen frühen explorativen C#-Prototypen als Referenz.

## Vorgeschlagene Projektstruktur

```text
SASD-Desktop-Secret-Manager/
├── assets/
│   └── readme/
├── docs/
│   ├── architektur/
│   ├── decisions/
│   ├── lastenheft/
│   └── pflichtenheft/
├── src/
│   ├── Sasd.SecretManager.Application/
│   ├── Sasd.SecretManager.Domain/
│   ├── Sasd.SecretManager.Interop.PasswordSafe/
│   ├── Sasd.SecretManager.Security/
│   ├── Sasd.SecretManager.Storage/
│   └── Sasd.SecretManager.WinForms/
├── tests/
│   ├── Sasd.SecretManager.Domain.Tests/
│   ├── Sasd.SecretManager.Interop.PasswordSafe.Tests/
│   ├── Sasd.SecretManager.Security.Tests/
│   └── Sasd.SecretManager.Storage.Tests/
└── tools/
```

## Sicherheitshinweis

Das Projekt ist sicherheitskritisch. Solange Kryptografie, Dateiformat, Tests, Recovery-Konzept, Import/Export und UI-Schutzmaßnahmen noch im Aufbau sind, dürfen keine echten produktiven Geheimnisse ausschließlich in frühen Entwicklungsständen verwaltet werden.

Details dazu stehen in [`SECURITY.md`](SECURITY.md).

## Nächste sinnvolle Schritte

1. Repository unter dem persönlichen GitHub-Account anlegen.
2. Diese Struktur einchecken.
3. Die Dokumente versionieren.
4. Lösung und Projektdateien anlegen.
5. Mit Domain-Modell, Security-Grundlagen und Storage-Format starten.

### License

This repository is licensed under the Apache License 2.0. The development repository currently lives under the personal GitHub account of Robin Goerlach. Selected release states may later be mirrored or transferred into repositories of SASD GmbH.
