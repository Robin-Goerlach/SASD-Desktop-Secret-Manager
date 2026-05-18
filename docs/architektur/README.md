# Architektur

Dieses Verzeichnis enthält das fortgeschriebene Architekturdokument.

## Führende Datei

- `Architekturdokument_SASD_Desktop_Secret_Manager.md`

Die frühere Kombination aus Lastenheft und Architekturkonzept wurde durch getrennte aktuelle Dokumente ersetzt. Für neue technische Entscheidungen ist dieses Architekturdokument zusammen mit ADRs, Security-/Threat-Model und Testkatalog führend.

## Aktualisierte Schwerpunkte

- Schichtentrennung Domain / Application / Security / Storage / Interop / UI / Tests
- `.svault` als internes Primärformat
- Password-Safe-Import als additive Interop-Schicht
- HTTPS-/TLS-Zertifikate als eigener Secret-Typ
- offene V1-Bausteine: Clipboard, Auto-Lock, Passwortgenerator, Import, Release-Härtung
