# Security

Dieses Repository enthält die konzeptionellen und technischen Grundlagen für einen sicherheitskritischen Desktop-Secret-Manager.

## Aktueller Status

Die Anwendung befindet sich noch im Aufbau. Frühere Entwicklungsstände dürfen **nicht** als alleinige produktive Geheimnisablage verwendet werden.

## Grundsätze

- keine Backdoor
- keine versteckten Recovery-Schlüssel
- keine Secrets im Quellcode
- keine Secrets in Logs
- keine unverschlüsselten Exportdateien ohne bewusste Nutzerentscheidung
- kein stillschweigendes Verwerfen von Interop-Daten

## Offen bekannte spätere Themen

- Recovery-Strategie ohne Backdoor
- Passwort-Historie
- ausgebauter Password-Safe-Kompatibilitätsmodus
- weitere Härtung im Speicher- und UI-Verhalten

## Meldung von Sicherheitsproblemen

Solange das Projekt noch im Aufbau ist, sollten Sicherheitsprobleme zunächst **vertraulich** und nicht öffentlich in Issues veröffentlicht werden.
