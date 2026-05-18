# DSM-001 – Passwortgenerator

## Ziel

Ein integrierter Passwortgenerator erzeugt starke Passwörter für neue oder bestehende Einträge.

## V1-Umfang

- Generator-Dialog aus Eintragsdialog aufrufbar.
- Länge konfigurierbar.
- Zeichengruppen wählbar: Großbuchstaben, Kleinbuchstaben, Zahlen, Sonderzeichen.
- Mindestlänge und sichere Defaults.
- Ergebnis kann in Secret-Feld übernommen werden.
- Keine automatische Speicherung ohne Nutzerbestätigung.

## Später

- Generatorprofile je Kontext.
- Eintragsvorlagen mit Generatorvorgaben.
- Ausschluss leicht verwechselbarer Zeichen.

## Tests

- Erzeugte Passwörter erfüllen gewählte Regeln.
- Keine leeren oder zu kurzen Ergebnisse.
- Generator nutzt kryptografisch geeignete Zufallsquelle.
- UI übernimmt Passwort nur nach bewusster Aktion.
