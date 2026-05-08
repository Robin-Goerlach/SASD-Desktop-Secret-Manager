# DSM-004 – Strukturierte Suche und Filter

## Ziel

Dieser Milestone ergänzt die vorhandene Volltextsuche um strukturierte Filter.
Damit kann die Eintragsliste gezielter eingeschränkt werden, ohne dass der
Benutzer eine Suchsyntax lernen muss.

## Umgesetzte Funktionen

- Filter nach Eintragstyp, zum Beispiel `Login`, `Database`, `Server`, `Mail` oder `ApiKey`
- Filter nach Tag
- Spezialfilter:
  - alle Einträge
  - ohne Gruppe
  - mit URL-Feld
  - mit Host-Feld
  - mit E-Mail-Feld
  - mit Zusatzfeldern
  - mit geheimen Zusatzfeldern
- Kombination mit der bestehenden Volltextsuche
- Kombination mit der bestehenden Gruppen-/Untergruppen-Auswahl im TreeView
- Reset-Funktion für Suchtext und strukturierte Filter
- zentrale Application-Logik in `VaultQueryService`
- neue Unit-Tests für die Filterlogik

## Architekturentscheidung

Die Filterlogik liegt weiterhin in der Application-Schicht. Die WinForms-UI
sammelt lediglich die Auswahl aus den ComboBoxen ein und erzeugt daraus ein
`EntryFilterCriteria`-Objekt. Dadurch bleibt die Logik unabhängig von der
Oberfläche und kann später für Importberichte, CLI-Werkzeuge oder andere UIs
wiederverwendet werden.

## Dateien

- `src/Sasd.SecretManager.Application/EntryFilterCriteria.cs`
- `src/Sasd.SecretManager.Application/EntrySpecialFilter.cs`
- `src/Sasd.SecretManager.Application/VaultQueryService.cs`
- `src/Sasd.SecretManager.WinForms/MainForm.cs`
- `tests/Sasd.SecretManager.Application.Tests/VaultQueryServiceFilterTests.cs`

## Tests

Nach dem Kopieren ausführen:

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src/Sasd.SecretManager.WinForms/Sasd.SecretManager.WinForms.csproj
```

## Manueller UI-Test

1. Anwendung starten.
2. Prüfen, ob oberhalb der Eintragsliste die Filterleiste sichtbar ist.
3. Typfilter auf `Database`, `Server` oder `ApiKey` setzen.
4. Tagfilter auf einen vorhandenen Tag setzen.
5. Spezialfilter testen, zum Beispiel `Mit URL-Feld` oder `Ohne Gruppe`.
6. Volltextsuche zusätzlich verwenden.
7. `Filter zurücksetzen` drücken.
8. Prüfen, ob die Eintragsliste wieder wie erwartet alle Einträge zeigt.

## Grenzen dieses Milestones

Dieser Milestone implementiert keine erweiterte Query-Sprache wie
`tag:prod type:server`. Das bleibt bewusst offen, weil einfache ComboBox-Filter
für V1 robuster und besser testbar sind.
