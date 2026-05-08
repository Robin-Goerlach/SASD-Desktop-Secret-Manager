# DSM-003 – Eintragsvalidierung und bessere Fehlermeldungen

## Ziel

Dieser Milestone führt eine zentrale fachliche Validierung für Einträge ein. Bisher wurden einzelne Regeln teilweise im Dialog geprüft oder beim Parsen stillschweigend toleriert. DSM-003 verschiebt die fachliche Entscheidung in die Application-Schicht und hält die WinForms-Oberfläche trotzdem benutzerfreundlich.

## Umgesetzte Punkte

- Neuer `EntryValidationService` in der Application-Schicht.
- Neues Ergebnisobjekt `EntryValidationResult` mit Fehlern und vorbereiteten Warnungen.
- Neue `EntryValidationException` für kontrollierte Validierungsabbrüche.
- Prüfung von Pflichtfeldern, Titellänge, Benutzernamenlänge und Secret-Länge.
- Prüfung unbekannter Gruppenpfade.
- Prüfung doppelter Eintragstitel innerhalb derselben Gruppe.
- Prüfung der Zusatzfeldsyntax `Name = Wert` oder `Name: Wert`.
- Prüfung doppelter Zusatzfeldnamen.
- Prüfung von Portnummern im Bereich 1 bis 65535.
- Vorsichtige Prüfung von URL- und E-Mail-Zusatzfeldern.
- Einbindung der Validierung in `EntryMutationService`.
- Benutzerfreundliche Warnmeldung im `EntryEditDialog`, bevor der Dialog geschlossen wird.
- Zusätzliche Behandlung tresorabhängiger Validierungsfehler in `MainForm`.
- Unit-Tests für zentrale Validierungsregeln.

## Bewusste Grenzen

- Die Validierung ist noch keine vollständige Security-Härtung.
- Secrets werden weiterhin als `string` im Domain-Modell gehalten.
- URL- und E-Mail-Prüfungen sind bewusst zurückhaltend, damit technische Spezialfälle nicht unnötig blockiert werden.
- Warnungen werden im Modell vorbereitet, aber noch nicht als eigener UI-Bereich dargestellt.

## Warum vor dem Password-Safe-Import?

Der spätere `.psafe3`-Import muss externe Daten in das interne Modell überführen. Eine zentrale Validierung ist dafür wichtig, weil Importdaten fehlerhaft, unvollständig oder uneindeutig sein können. DSM-003 schafft dafür die Grundlage.

## Testempfehlung

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src/Sasd.SecretManager.WinForms/Sasd.SecretManager.WinForms.csproj
```

Manuell prüfen:

1. Neuen Eintrag ohne Titel speichern → Warnmeldung im Dialog.
2. Zusatzfeld `Host db.example.org` speichern → Warnmeldung zur Syntax.
3. Zusatzfeld `Port = 70000` speichern → Warnmeldung zur Portnummer.
4. Zwei Einträge mit gleichem Titel in derselben Gruppe anlegen → Warnmeldung nach dem Dialog.
5. Zwei Einträge mit gleichem Titel in unterschiedlichen Gruppen anlegen → erlaubt.
