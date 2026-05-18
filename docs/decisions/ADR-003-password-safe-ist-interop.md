# ADR-003: Password Safe ist Interop, nicht Produktkern

## Status

Akzeptiert

## Kontext

Bestehende Password-Safe-Daten sollen übernommen werden können. Gleichzeitig ist das Zielprodukt fachlich breiter als Password Safe.

## Entscheidung

Password Safe wird über Import und später begrenzten Export unterstützt. Es wird nicht zum funktionalen Obermodell des Produkts.

## Konsequenzen

- V1 priorisiert kontrollierten Import.
- Export folgt erst nach Mapping- und Verlustkonzept.
- Roundtrip-Kompatibilität ist kein V1-Ziel.
- Importberichte sind Pflicht.
