# ADR-007: Security-/Threat-Model als Release-Gate

## Status

Akzeptiert

## Kontext

Ein Secret Manager ist sicherheitskritisch. Ein Release darf nicht nur funktional, sondern muss auch sicherheitsbezogen verantwortbar sein.

## Entscheidung

Das Security-/Threat-Model ist ein verbindliches Release-Gate für V1.

## Konsequenzen

- Neue sicherheitsrelevante Features werden gegen das Threat Model geprüft.
- Clipboard, Auto-Lock, Logging, Backups, Import und Zertifikate benötigen Tests.
- Offene kritische Security-Punkte blockieren V1.
