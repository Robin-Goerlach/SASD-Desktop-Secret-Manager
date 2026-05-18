# Development Setup

## Ziel

Dieses Dokument beschreibt den lokalen Entwicklungsstart für den SASD Desktop Secret Manager.

## Voraussetzungen

- Windows 10 oder Windows 11
- .NET SDK 8.x
- Visual Studio 2022 oder Visual Studio Code
- Git
- optional: PowerShell 7

## Projekt wiederherstellen, bauen und testen

```bash
dotnet restore
dotnet build
dotnet test
```

## Anwendung starten

```bash
dotnet run --project src/Sasd.SecretManager.WinForms/Sasd.SecretManager.WinForms.csproj
```

## Empfohlener Entwicklungsablauf

1. Vor Änderungen `git status` prüfen.
2. Kleine Funktionspakete bearbeiten.
3. Nach jedem Paket `dotnet build` und möglichst `dotnet test` ausführen.
4. Bei sicherheitsrelevanten Änderungen Security-/Threat-Model und Testkatalog aktualisieren.
5. Commit mit fachlich sprechender Nachricht erstellen.

## Sicherheitsregeln im Entwicklungsbetrieb

- Keine echten produktiven Passwörter, Tokens oder Zertifikats-Private-Keys in Testdaten verwenden.
- Keine `.svault`-Dateien mit echten Secrets ins Repository aufnehmen.
- Keine Klartext-Exports, CSV-Dateien oder Zertifikatsdateien versehentlich commiten.
- Debug-Logs regelmäßig auf Secret-Leaks prüfen.
- Testdaten sollen realistisch strukturiert, aber künstlich sein.

## Häufige Befehle

```bash
# vollständiger Prüfablauf
dotnet restore && dotnet build && dotnet test

# nur Tests eines Projekts
dotnet test tests/Sasd.SecretManager.Security.Tests/Sasd.SecretManager.Security.Tests.csproj

# Status prüfen
git status
```

## Dokumentationsbezug

Neue Features sollten gegen folgende Dokumente geprüft werden:

- `docs/roadmap.md`
- `docs/featuremap/Featuremap_SASD_Desktop_Secret_Manager.md`
- `docs/security/Security_Threat_Model_SASD_Desktop_Secret_Manager.md`
- `docs/tests/Test_und_Abnahmekatalog_V1_SASD_Desktop_Secret_Manager.md`
