# DSM-007 – Password-Safe-Import

## Ziel

Vorhandene `.psafe3`-Daten sollen kontrolliert in das interne `.svault`-Modell übernommen werden.

## V1-Umfang

- Import-Dialog oder Import-Assistent.
- `.psafe3`-Datei auswählen.
- Master-Passwort abfragen.
- Mapping auf interne Einträge.
- Importbericht mit Warnungen.
- Kein stiller Datenverlust.

## Nicht-Ziele V1

- Vollständiger Roundtrip.
- `.psafe3` als primäres Arbeitsformat.
- Automatischer Export.

## Tests

- Erfolgreicher Import.
- Falsches Passwort.
- Beschädigte Datei.
- Sonderzeichen/Umlaute.
- Gruppenstruktur.
- Warnung bei nicht abbildbaren Feldern.
