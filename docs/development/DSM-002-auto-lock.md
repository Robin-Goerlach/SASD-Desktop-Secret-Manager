# DSM-002 – Auto-Lock / Tresor sperren

## Ziel

Dieser Milestone ergänzt eine erste Auto-Lock- und Sperr-Funktion für den SASD Desktop Secret Manager.
Der Tresor kann manuell über `Datei → Tresor sperren` gesperrt und anschließend über denselben Menüpunkt wieder entsperrt werden.
Zusätzlich prüft ein WinForms-Timer regelmäßig die letzte Benutzeraktivität und sperrt den Tresor nach Inaktivität automatisch.

## Inhalt

- neue Application-Logik `AutoLockOptions`
- neue Application-Logik `AutoLockService`
- Tests für die Auto-Lock-Entscheidung
- vollständige `MainForm.cs` mit manueller Sperre, Entsperren und Auto-Lock-Timer
- ergänzte `DevLog.Info(...)`-Methode
- zusätzliche Passwortgenerator-Logmeldungen

## Sicherheitsgrenzen dieses Milestones

Dies ist eine V1-nahe UI-Sperre. Sichtbare Secrets, Eintragslisten und Gruppenbaum werden ausgeblendet bzw. deaktiviert.
Bei sauber gespeicherten Tresoren kann das Entsperren über das echte Tresorformat geprüft werden.
Bei neuen oder ungespeicherten Tresoren bleibt der In-Memory-Stand erhalten, damit Auto-Lock keine Daten verwirft.

Eine noch strengere spätere Variante könnte zusätzlich den In-Memory-Tresor verwerfen und ausschließlich aus der verschlüsselten Datei neu laden.
Das sollte aber erst umgesetzt werden, wenn Save-/Dirty-State, Recovery und UX vollständig sauber sind.

## Manuelle Prüfung

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src/Sasd.SecretManager.WinForms/Sasd.SecretManager.WinForms.csproj
```

Danach prüfen:

1. `Werkzeuge → Passwortgenerator` öffnet den Generator und schreibt Debug-Logs.
2. `Datei → Tresor sperren` sperrt die Oberfläche.
3. Der Menüpunkt heißt danach `Tresor entsperren`.
4. Nach Eingabe des Master-Passworts wird der Tresor wieder freigegeben.
5. Nach Inaktivität wird automatisch gesperrt.

## Commit-Vorschlag

```powershell
git add .
git commit -m "Add vault auto lock milestone"
```
