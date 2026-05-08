# DSM-001 – Passwortgenerator

## Ziel

Dieser Milestone ersetzt den bisherigen Platzhalter „Passwortgenerator“ durch eine erste nutzbare V1-Funktion.

## Umgesetzte Punkte

- Neuer `PasswordGeneratorService` in der Application-Schicht.
- Kryptographisch sicherer Zufall über `RandomNumberGenerator`.
- Optionen für Länge, Großbuchstaben, Kleinbuchstaben, Ziffern und Sonderzeichen.
- Option zum Vermeiden verwechselbarer Zeichen.
- Option, aus jeder gewählten Zeichengruppe mindestens ein Zeichen zu erzwingen.
- WinForms-Dialog zum Generieren, Kopieren und Übernehmen eines Passworts.
- Integration in den Eintragsdialog: generiertes Passwort kann direkt in das Secret-Feld übernommen werden.
- Unit-Tests für Kernverhalten.

## Bewusst noch nicht enthalten

- Globale Clipboard-Auto-Clear-Integration im Generator-Dialog.
- Passwort-Historie.
- Generatorprofile pro Gruppe oder Eintragstyp.
- Passphrase-/Wortlistenmodus.

Diese Punkte passen besser in spätere Milestones.
