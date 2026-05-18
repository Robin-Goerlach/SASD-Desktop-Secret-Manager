# DSM-004 – Strukturierte Filter

## Ziel

Einträge sollen über Suche, Typ und Tags zuverlässig auffindbar sein.

## V1-Umfang

- Suche über Titel, Benutzer/Principal, Notizen, Tags, Gruppenpfad und nicht geheime Zusatzfelder.
- Typfilter für EntryType.
- Tagfilter für bekannte Tags.
- Case-insensitive Standardsuche.
- Root-Ansicht bleibt übersichtlich.

## Später

- Filterkombinationen speichern.
- Such-Highlighting.
- Erweiterte Suche mit Fallunterscheidung.
- Multi-Vault-Suche.

## Tests

- Suche findet Tags.
- Suche findet CustomFields.
- Secret-Felder werden nicht unkontrolliert in Klartextindex ausgelagert.
- Typfilter und Tagfilter kombinieren korrekt.
