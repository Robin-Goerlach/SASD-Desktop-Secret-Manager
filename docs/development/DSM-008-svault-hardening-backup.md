# DSM-008 – `.svault`-Hardening und Backup

## Ziel

Speichern und Laden sollen robust gegen Datenverlust, beschädigte Dateien und Bedienfehler sein.

## V1-Umfang

- Atomisches Speichern.
- Backup vor Überschreiben.
- Klare Fehler bei falschem Passwort, beschädigter Datei und Zugriffskonflikt.
- Kein stilles Akzeptieren unvollständiger Container.
- KDF-/Formatparameter im Container prüfen.

## V1.x

- Zusätzliche Sicherung unter `%LocalAppData%`.
- Rotierende Backups.
- Bessere Restore-UI.

## Tests

- Speichern und Wiederöffnen.
- Abgeschnittene Datei.
- Manipulierter Header.
- Falscher Auth-Tag.
- Backup wird erzeugt und bleibt verschlüsselt.
