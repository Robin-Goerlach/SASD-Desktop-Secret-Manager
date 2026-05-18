# DSM-002 – Auto-Lock

## Ziel

Der geöffnete Tresor wird nach Inaktivität gesperrt, damit Secrets nicht dauerhaft sichtbar bleiben.

## V1-Umfang

- Manuelles Sperren des Tresors.
- Automatisches Sperren nach konfigurierbarer Inaktivitätszeit.
- Entsperren nur mit Master-Passwort des aktiven Tresors.
- Sichtbare Secret-Felder werden beim Lock maskiert oder geleert.
- Clipboard sollte beim Lock für sensible Werte bereinigt werden.

## Nicht-Ziele V1

- Windows-Hello-Entsperrung.
- Multi-User-Sitzungen.
- Remote-Lock.

## Tests

- Auto-Lock löst nach Inaktivität aus.
- Manuelles Lock funktioniert.
- Falsches Passwort entsperrt nicht.
- Nach Lock sind Secrets nicht sichtbar.
