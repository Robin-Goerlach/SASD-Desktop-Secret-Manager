# ADR-002: `.svault` ist Primärformat

## Status

Akzeptiert

## Kontext

Das Produkt benötigt ein eigenes versionsfähiges Format für strukturierte Secrets, Zusatzfelder, Zertifikate, Metadaten und spätere Migrationen.

## Entscheidung

`.svault` ist das interne, verschlüsselte Primärformat des Produkts.

## Konsequenzen

- Password Safe bleibt Interop, nicht Primärspeicher.
- Header, Version, KDF-Parameter und verschlüsselte Nutzlast werden sauber getrennt.
- Migrationen werden explizit eingeplant.
- Backups bleiben verschlüsselt.
