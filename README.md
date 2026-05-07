# SASD Desktop Secret Manager

Technisch sauber aufgebauter, lokal orientierter Desktop-Secret-Manager für Windows auf Basis von **C# / .NET 8** und **WinForms**.
Das Projekt wird aktuell als persönliches Entwicklungs-Repository geführt. Spätere, bereinigte Release-Stände können separat in ein SASD-GmbH-Repository übernommen werden.

> **Status:** Aktiver Entwicklungsstand mit lauffähigem WinForms-Prototyp, sauberer Projektstruktur, grünem Build-/Test-Stand und ausführlich kommentiertem Code. Noch **nicht** als produktives Sicherheitsprodukt freigeben.

## Visuelle Richtung

![Konzept-Screenshot der Anwendung](assets/readme/app-concept.png)

Die Anwendung folgt einer ruhigen Desktop-Oberfläche mit Drei-Spalten-Layout:

- links Tresor-, Gruppen- und Navigationsbereich,
- mittig Such- und Ergebnisbereich,
- rechts Detailansicht des ausgewählten Eintrags.

## Projektidee

Die Anwendung soll **mehr sein als ein einfacher Passwortmanager**.
Ziel ist ein lokaler Secret Manager mit verschlüsselten Tresoren, der neben klassischen Logins auch technische Zugänge strukturiert verwalten kann, zum Beispiel:

- Hosting-Backends
- Datenbankverbindungen
- FTP/SFTP-Zugänge
- Mailkonten
- API-Tokens und technische Secrets
- GitHub-Zugänge
- Server- und Infrastruktur-Zugänge
- weitere technische Zusatzinformationen über flexible Zusatzfelder

## Leitlinien

- **internes Datenmodell** als fachlich führende Struktur
- **mehrere unabhängige Tresore**
- **Gruppen und Tags** zur Organisation
- **starke Verschlüsselung** und klare Sicherheitsgrenzen
- **Password Safe Interop** als zusätzliche Kompatibilitätsschicht, nicht als Primärformat
- saubere Trennung von **Domain**, **Application**, **Security**, **Storage**, **Interop** und **UI**
- **ausführlich kommentierter Code** mit XML-Dokumentation und erklärenden Kommentaren

## Aktuell umgesetzter Entwicklungsstand

Der aktuelle Stand deckt wesentliche Teile des geplanten V1-Kerns bereits ab:

- lauffähige WinForms-Anwendung auf .NET 8
- internes Tresorformat (`.svault`) mit verschlüsseltem Speichern und Laden
- Gruppen, Untergruppen und Tags
- Suche, Listenansicht und Detailansicht
- Einträge anlegen, bearbeiten und löschen
- Gruppen anlegen, umbenennen und löschen (mit Schutzlogik)
- Drag & Drop für Einträge und Gruppen
- Rückfragen bei riskanteren Organisationsoperationen
- Passwortstärke-Bewertung für Master-Passwörter
- Debug-Logging für Entwicklungs- und Testzwecke
- automatisierte Tests für Domain-, Application-, Security-, Storage- und Interop-Grundlogik

## Noch nicht abgeschlossene Themen bis zu einer runden V1

Einige fachlich wichtige Punkte sind bewusst noch offen oder nur teilweise begonnen, zum Beispiel:

- Passwortgenerator
- Auto-Lock / Sperren des Tresors
- Clipboard-Autoclear für sensible Copy-Aktionen
- weiter ausgebauter `.psafe3`-Import
- zusätzliche Härtungs-, Komfort- und Release-Themen

Die vollständige fachliche Einordnung steht in Lastenheft, Pflichtenheft und Architekturdokument.

## Repository-Strategie

Aktuell ist eine **Entwicklung unter dem persönlichen GitHub-Account** vorgesehen.
Zwischenschritte, Experimente und Lernfortschritte können dort bleiben. Später können bereinigte Release-Stände in ein separates Repository unter `sasdgmbh` übernommen oder gespiegelt werden.

Mehr dazu in:

- [`docs/repository-strategy.md`](docs/repository-strategy.md)
- [`docs/repository-metadata.md`](docs/repository-metadata.md)

## Dokumente

- [`docs/lastenheft/`](docs/lastenheft/)
- [`docs/pflichtenheft/`](docs/pflichtenheft/)
- [`docs/architektur/`](docs/architektur/)
- [`docs/roadmap.md`](docs/roadmap.md)
- [`docs/decisions/`](docs/decisions/)

## Projektstruktur

```text
SASD-Desktop-Secret-Manager/
├── assets/
│   └── readme/
├── docs/
│   ├── architektur/
│   ├── decisions/
│   ├── lastenheft/
│   ├── pflichtenheft/
│   └── README.md
├── src/
│   ├── Sasd.SecretManager.Application/
│   ├── Sasd.SecretManager.Domain/
│   ├── Sasd.SecretManager.Interop.PasswordSafe/
│   ├── Sasd.SecretManager.Security/
│   ├── Sasd.SecretManager.Storage/
│   └── Sasd.SecretManager.WinForms/
├── tests/
│   ├── Sasd.SecretManager.Application.Tests/
│   ├── Sasd.SecretManager.Domain.Tests/
│   ├── Sasd.SecretManager.Interop.PasswordSafe.Tests/
│   ├── Sasd.SecretManager.Security.Tests/
│   └── Sasd.SecretManager.Storage.Tests/
├── tools/
│   └── README.md
├── Directory.Build.props
├── global.json
└── SASD-Desktop-Secret-Manager.sln
```

## Build und Test

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/Sasd.SecretManager.WinForms/Sasd.SecretManager.WinForms.csproj
```

## Sicherheitshinweis

Das Projekt ist sicherheitskritisch.
Solange Kryptografie, Dateiformat, Recovery-Konzept, Import/Export, Clipboard-Schutz und weitere Härtungspunkte noch nicht vollständig abgeschlossen sind, dürfen keine echten produktiven Geheimnisse ausschließlich in frühen Entwicklungsständen verwaltet werden.

Details dazu stehen in [`SECURITY.md`](SECURITY.md).

## Lizenz

Dieses Repository steht unter der **Apache License 2.0**.
Das Entwicklungs-Repository liegt derzeit unter dem persönlichen GitHub-Account von Robin Goerlach. Ausgewählte Release-Stände können später in Repositories der SASD GmbH gespiegelt oder übertragen werden.
