# ADR-005: Keine Recovery-Backdoor in V1

## Status

Akzeptiert

## Kontext

Ein Recovery-Mechanismus kann nützlich sein, birgt aber bei Passwortmanagern erhebliche Risiken.

## Entscheidung

V1 enthält keine versteckte Recovery-Backdoor. Wer das Master-Passwort verliert, darf nicht über einen geheimen Entwickler- oder Herstellerweg an die Daten gelangen.

## Konsequenzen

- Recovery wird nur als späteres explizites Konzept betrachtet.
- Keine Hintertür in Dateiformat oder Kryptografie.
- Dokumentation muss diese Grenze ehrlich benennen.
