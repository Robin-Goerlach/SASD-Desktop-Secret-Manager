# DSM-005 – Clipboard Protection

## Ziel

Sensible Copy-Aktionen dürfen Secrets nicht dauerhaft in der Zwischenablage belassen.

## V1-Umfang

- Zentraler ClipboardProtectionService.
- Sensible und nicht sensible Copy-Aktionen unterscheiden.
- Sensible Inhalte nach konfigurierbarer Zeit löschen.
- Statusfeedback nach Copy und nach Autoclear.
- Kein Secret-Inhalt in Logs oder Statusmeldungen.

## Sonderfälle

Wenn der Nutzer die Zwischenablage nach dem Kopieren überschreibt, darf die Anwendung nicht fremde Inhalte löschen. Autoclear soll nur löschen, wenn noch der von der Anwendung gesetzte sensible Wert vorhanden ist.

## Tests

- Sensibles Passwort wird kopiert und später gelöscht.
- Nicht sensibles Feld wird nicht unnötig gelöscht.
- Überschriebene Zwischenablage wird nicht zerstört.
- Statusmeldungen enthalten keine Secret-Werte.
